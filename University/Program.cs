
using Microsoft.EntityFrameworkCore;
using University.Domain.Contracts;
using University.Domain.Entities;
using University.Domain.Presistent;
using University.Infrasructure.Repositories;
using University.Infrasructure.Services;
using System.Reflection;
using University.Infrasructure.Models;

namespace University;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<UniversityDbContext>(options => options.UseSqlServer
            (builder.Configuration.GetConnectionString("UniversityDbContext"), b => b.MigrationsAssembly("University.Infrasructure")));

        builder.Services.AddScoped<IRepository<Course>, CourseRepository>();
        builder.Services.AddScoped<IRepository<Group>, GroupRepository>();
        builder.Services.AddScoped<IRepository<Student>, StudentRepository>();
        builder.Services.AddScoped<ICourseService, CourseService>();
        builder.Services.AddScoped<IGroupService, GroupService>();
        builder.Services.AddScoped<IStudentService, StudentService>();
        builder.Services.AddScoped<CourseModel>();
        builder.Services.AddScoped<GroupModel>();
        builder.Services.AddAutoMapper(Assembly.Load("University.Infrasructure"));

        var app = builder.Build();


        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");

            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Courses}/{action=ListEntities}/{Id?}");

        app.Run();
    }
}