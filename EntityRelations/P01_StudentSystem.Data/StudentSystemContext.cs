using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {

        }

        public StudentSystemContext(DbContextOptions options)
            :base(options)
        {

        }

        public DbSet<Course> Courses  { get; set; }

        public DbSet<Homework> Homeworks  { get; set; }

        public DbSet<Resource> Resources  { get; set; }

        public DbSet<Student> Students  { get; set; }

        public DbSet<StudentCourse> StudentCourses  { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(c => c.Name)
                .IsUnicode(true);

                entity.Property(c => c.Description)
                .IsUnicode(true);

                

            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(x => x.Name)
                .IsUnicode(true);

               entity.Property(s => s.PhoneNumber)
              .IsRequired(false)
              .IsUnicode(false)
              .HasColumnType("CHAR(10)");

            });

            modelBuilder.Entity<Homework>(entity =>
            {
               

                entity
                    .HasOne(h => h.Student)
                    .WithMany(s => s.HomeworkSubmissions)
                    .HasForeignKey(h => h.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(h => h.Course)
                    .WithMany(c => c.HomeworkSubmissions)
                    .HasForeignKey(h => h.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.Property(r => r.Name)
                .IsUnicode(true);

                entity.HasOne(r => r.Course)
               .WithMany(c => c.Resources)
               .HasForeignKey(r => r.CourseId)
               .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity.HasKey(sc => new {sc.StudentId , sc.CourseId });

                entity.HasOne(sc => sc.Course)
               .WithMany(c => c.StudentsEnrolled)
               .HasForeignKey(sc => sc.CourseId);

                entity.HasOne(sc => sc.Student)
              .WithMany(s => s.CourseEnrollments)
              .HasForeignKey(sc => sc.StudentId);

            });


        }

    }
}
