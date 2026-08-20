# Staff Portal RBAC — how it works

Reference doc for the per-page permission system added to the Staff portal. If you're extending
this feature, changing what a permission gates, or debugging why a staff user can/can't see a
page, start here.

## The problem this solves

Before this feature, every Staff-portal user could see and call every staff page/endpoint. The
only access boundary was Student vs. Staff (`[Authorize(Roles = "Staff")]`). There was no way for
an Admin to restrict which staff pages a given employee — a teacher, a front-desk staff member,
etc. — could use.

## The model

- **4 grantable permission keys**, one per non-Dashboard staff nav item: `ActionHub`, `Students`,
  `Programs`, `Messages`. Dashboard itself is never gated — every staff user always sees it.
- **`StaffRole.Admin` always has all 4, implicitly.** Admins never get rows in the
  `StaffPermissions` table. This is a claims-based bypass, checked at two points (see below), not
  a stored grant — so an Admin can never be "locked out" by someone editing a table, and no
  combination of grantable permissions can ever produce Admin-equivalent access.
- **Only `StaffRole.Admin` can reach the permission-management screen itself**
  (`/staff/permissions`, backed by `StaffManagementController`). This is gated by `StaffRole`
  directly (`RequireStaffRoleAttribute`), never by a grantable permission — otherwise a non-admin
  could theoretically be granted a permission that lets them grant themselves Admin.
- **Revoking a permission hard-deletes the row.** There's no soft-delete (`IsActive` flag) path
  for `StaffPermission` — this codebase has no global EF query filter, so a soft-deleted row would
  silently still count as granted anywhere it's read unless every call site remembered to filter
  it out. Hard delete removes that whole class of bug.

## Data model

```
StaffUser (existing)
  └─ Permissions: ICollection<StaffPermission>   (new navigation)

StaffPermission (new entity, DigitalSchoolManagementSystem.Domain/Entities/StaffPermission.cs)
  - StaffUserId (FK, cascade delete with the StaffUser)
  - PermissionKey (string, one of StaffPermissionKeys.All)
  - unique index on (StaffUserId, PermissionKey)
```

Permission keys are plain `string` constants
(`DigitalSchoolManagementSystem.Domain/Constants/StaffPermissionKeys.cs`), not a C# enum. This
codebase's enums serialize numerically (no `JsonStringEnumConverter`), but these keys need to be
stable literal strings shared verbatim with the frontend (`session.permissions.includes(key)`).
Mirrored on the frontend as `StaffPermissionKey` in `src/types/enums.ts`.

## How a permission check happens

**Backend** — two custom `IAuthorizationFilter` attributes in
`DigitalSchoolManagementSystem.API/Authorization/`:

- `[RequireStaffPermission(StaffPermissionKeys.X, ...)]` — OR semantics (any listed key is
  enough). Bypassed if the `staffRole` claim on the token equals `StaffRole.Admin`. Otherwise
  requires a matching `permission` claim. Stacked alongside `[Authorize(Roles = "Staff")]` on the
  action, not instead of it.
- `[RequireStaffRole(StaffRole.Admin)]` — no bypass, just checks the `staffRole` claim is in the
  allowed list. Applied at the class level on `StaffManagementController`.

Both claims (`staffRole`, one `permission` claim per granted key) are added in
`JwtTokenService.GenerateAccessToken` — **raw grants only, no Admin-bypass baked into the token
itself.** The token/`AuthResponseDto.Permissions` always reflect literal `StaffPermission` rows
(empty for Admins), so there's exactly one place per side (this attribute; the frontend helper
below) that knows "Admin implies everything" — it's never silently smeared into transport data.

**Frontend** — `src/utils/permissions.ts`:

- `isStaffAdmin(session)` — `session.staffRole === StaffRole.Admin`.
- `hasStaffPermission(session, key)` — `true` if `isStaffAdmin`, else
  `session.permissions.includes(key)`.

Exposed via `useAuth()` as `isAdmin` / `hasPermission(key)`. `StaffLayout.tsx` filters nav items
with `hasPermission`; `PermissionRoute` (route guard, mirrors the existing `RoleRoute` pattern)
blocks direct navigation to a page's URL; `AdminRoute` gates `/staff/permissions` on `isAdmin`
specifically, the same StaffRole-not-permission rule as the backend.

**The frontend gate is UX only.** Every gated action re-checks server-side regardless — a user
editing `localStorage` or calling the API directly gets a real `403`, not just a hidden button.

## Propagation timing

Permission changes take effect **on the affected user's next access-token refresh or next
login** — not instantly. The access token is a snapshot of grants at mint time (default 30 min
lifetime, see `Jwt:AccessTokenExpirationMinutes`). This matches how the existing `Role` claim
already worked before this feature; nothing new was introduced here. If you need instant
revocation for a specific incident, the blunt tool is deleting the user's `RefreshToken` rows to
force a fresh login next time their access token expires.

## Endpoint → permission mapping

| Permission | Gates |
| --- | --- |
| `ActionHub` | `ProgramsController.GetPendingApplications`, `DocumentsController.GetPending`, `DocumentsController.Review` |
| `Students` | `StudentsController` (`GetAll`, `GetById`, `GetEducationStatus`, `GetAcademics`, `Update`, `Delete`), `DocumentsController.GetByUser` |
| `Programs` | `ProgramsController` (`Create`, `Update`, `UpdateStatus`, `GetApplications`) |
| `ActionHub` **or** `Programs` | `ProgramsController.ReviewApplication` — shared by the Action Hub's approve/reject and a program's own detail-page applications tab; either permission is enough |
| `Messages` | `ConversationsController.UpdateStatus` (resolving/closing a Query) |

`AttendanceController`, `ExamsController`, `ExamResultsController`, `SubjectsController` are
**not** gated — they have staff CRUD but no dedicated frontend page reaches them (only
summary-level data is surfaced elsewhere), so they were out of scope.

## Known limitation: messaging isn't fully lockable

`ConversationsController`'s base endpoints (`GetMyConversations`, `SendMessage`, `StartDirect`,
etc.) are `[Authorize]`-only, shared by both the Student and Staff portals with no staff-specific
restriction — adding a `Messages` permission check there would also block students, who use the
same endpoints. Only `UpdateStatus` (a staff-only action: resolving/closing a Query) is gated. So
revoking `Messages` reliably hides the Messages page/nav and blocks query resolution, but a staff
user without it could still technically call the shared conversation endpoints directly. Fixing
this properly would mean splitting staff-specific messaging endpoints out from the shared ones —
out of scope for this pass.

## Migration & backfill

Migration: `20260819170351_AddStaffPermissions`. Every staff user that existed **before** this
migration was backfilled with all 4 permissions (preserves their existing access — the day this
shipped wasn't meant to lock anyone out). Staff registered **after** this migration start with
zero permissions until an Admin grants them. See the migration's `Up()` for the backfill SQL if
you need to replicate this pattern for a future permission key.

## Adding a 5th permission key

1. Add the constant to `StaffPermissionKeys` (backend) and `StaffPermissionKey` (frontend,
   `src/types/enums.ts`).
2. Add a label for it in `StaffManagementService.PermissionLabels`.
3. Apply `[RequireStaffPermission(StaffPermissionKeys.NewKey)]` to the relevant controller
   action(s), alongside the existing `[Authorize(Roles = "Staff")]`.
4. Add the nav entry + a `permissionKey` on it in `StaffLayout.tsx`, and wrap its route(s) in
   `<PermissionRoute permission={StaffPermissionKey.NewKey}>` in `App.tsx`.
5. No migration needed — `StaffPermission.PermissionKey` is a free-text column, not an enum/CHECK
   constraint. `StaffManagementService.SetPermissionsAsync` validates incoming keys against
   `StaffPermissionKeys.All`, so step 1 is what makes the new key acceptable.

## Verification performed

Confirmed end-to-end against the real API and LocalDB (not just build/type-check):

- Fresh backend build (`dotnet build`) and frontend build/type-check/lint
  (`npx tsc -b`, `npm run build`, `npm run lint`) all clean.
- Migration applied; pre-existing staff correctly backfilled with all 4 permissions each, 0 rows
  for Admins.
- A newly-registered non-Admin staff user starts with `permissions: []` and gets `403` on
  `/api/students`, `/api/programs/applications/pending`, and `/api/staff-management/staff`.
- An Admin account bypasses via the `staffRole` claim alone (zero `StaffPermission` rows) and can
  reach `/api/staff-management/*`; a non-Admin staff token gets `403` there.
- Granting `Students` via `PUT /api/staff-management/staff/{id}/permissions`, then refreshing the
  target user's token (not just re-logging-in) correctly picks up the new grant — this
  specifically exercises the `RefreshTokenRepository` include-chain fix, which was the one gap
  found during implementation (permissions weren't being loaded on the refresh path).
- OR-semantics on `ReviewApplication` confirmed directly: granting only `Programs` (no
  `ActionHub`) reaches the action (`404` on a bogus application id, i.e. past the permission
  check); granting only `Students` (neither) is blocked (`403`).
- Revoke hard-deletes the DB row (verified directly in LocalDB) and access reverts to `403` on the
  next token refresh.
- Admin self-lockout guard: attempting to set explicit permissions on an Admin account returns
  `409` with a clear message, not a silent no-op. An unknown permission key returns `400`.

**Not verified visually in a browser** (no browser-automation tool was available in the session
that built this) — the nav filtering, the checkbox-matrix admin screen's rendering/interaction,
and toast feedback are implemented per the existing codebase's component patterns and pass
type-checking/build/lint, but weren't click-tested. Worth a manual pass before considering this
fully signed off.

### Test accounts left in the dev DB from verification

- `rbactest_admin1` / `TestPass123!` — **StaffRole.Admin**. Useful as-is: it's the only Admin
  account that existed in the local DB at the time of this change (every other seeded/registered
  staff account is `Staff` or `Teacher`), so this is currently the only way to log in and see the
  new `/staff/permissions` screen locally. Delete or repurpose once you've promoted/created your
  own Admin account.
- `rbactest_staff1` / `TestPass123!` — plain `Staff`, currently has zero permissions (used to
  exercise the grant/revoke cycle above; left at its post-revoke state).
