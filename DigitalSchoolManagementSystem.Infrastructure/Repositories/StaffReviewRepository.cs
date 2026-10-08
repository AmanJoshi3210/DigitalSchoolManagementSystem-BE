using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Entities;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class StaffReviewRepository : IStaffReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public StaffReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public  async Task<IEnumerable<ReviewStaff?>> GetAllReviews()
        {
            return _context.ReviewStaffs.Where(a => a.IsActive == true);
        }

        public async Task<ReviewStaff?> GetReviewById(int id)
        {
            return _context.ReviewStaffs.FirstOrDefault(a => a.Id == id && a.IsActive == true);
        }

        public async Task<ReviewStaff?> AddReview(ReviewStaff review)
        {
            _context.ReviewStaffs.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<IEnumerable<ReviewerStudent?>> GetReviewerStudentsAsync(int reviewId)
        {
            return _context.ReviewerStudents.Where(a => a.ReviewStaffId == reviewId && a.IsActive == true);
        }

        public async Task<ReviewerStudent?> GetReviewerStudentByIdAsync(int id)
        {
            return _context.ReviewerStudents.FirstOrDefault(a => a.Id == id && a.IsActive == true);
        }

        public async Task<ReviewerStudent?> AddReviewerStudentAsync(ReviewerStudent reviewerStudent)
        {
            _context.ReviewerStudents.Add(reviewerStudent);
            await _context.SaveChangesAsync();
            return reviewerStudent;
        }
    }
}
