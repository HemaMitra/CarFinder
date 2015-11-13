using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace CarFinder.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Car> Cars { get; set; }

        /// <summary>
        /// Get all the years for which cars are listed in the database.
        /// </summary>
        /// <returns>A list of years</returns>
        //Get All Years
        public async Task<List<string>> AllYears()
        {
            return await this.Database.SqlQuery<string>("AllYears").ToListAsync();
        }

        // Get Makes By Year

        public async Task<List<string>> MakesByYear(string year)
        {
            var yearParm = new SqlParameter("@year", year); 
            return await this.Database.SqlQuery<string>("MakesByYear @year",yearParm).ToListAsync();
        }

        // Get Models By Year And Make

        public async Task<List<string>> ModelsByYearMake(string year, string make)
        { 
            var yearParm = new SqlParameter("@year",year);
            var makeParm = new SqlParameter("@make",make);
            return await this.Database.SqlQuery<string>("ModelsByYearMake @year,@make", yearParm, makeParm).ToListAsync();
        }

        // Get Trims By Year, Make and Model
        public async Task<List<string>> Trims(string year, string make, string model)
        {
            var yearParm = new SqlParameter("@year", year);
            var makeParm = new SqlParameter("@make", make);
            var modelParm = new SqlParameter("@model",model);
            return await this.Database.SqlQuery<string>("Trims @year,@make,@model", yearParm, makeParm,modelParm).ToListAsync();
        }

        // Get Cars By Year
        public async Task<List<Car>> CarsByYear(string year)
        {
            var yearParm = new SqlParameter("@year",year);

            return await this.Database.SqlQuery<Car>("CarsByYear @year",yearParm).ToListAsync();
        }

        // Get Cars By Year and Make

        public async Task<List<Car>> CarsByYearMake(string year, string make)
        {
            var yearParm = new SqlParameter("@year",year);
            var makeParm = new SqlParameter("@make", make);

            return await this.Database.SqlQuery<Car>("CarsByYearMake @year,@make", yearParm, makeParm).ToListAsync();
        }

        // Get Cars By Year , Make and Model

        public async Task<List<Car>> CarsYearMakeModel(string year, string make, string model)
        {
            var yearParm = new SqlParameter("@year",year);
            var makeParm = new SqlParameter("@make", make);
            var modelParm = new SqlParameter("@model",model);

            return await this.Database.SqlQuery<Car>
                ("CarsYearMakeModel @year,@make,@model", yearParm, makeParm, modelParm).ToListAsync();
        }

        // Get Cars by Year, Make, Model and Trim
        public async Task<List<Car>> CarsYearMakeModelTrim(string year, string make, string model, string trim)
        {
            var yearParm = new SqlParameter("@year", year);
            var makeParm = new SqlParameter("@make", make);           
            var trimParm = new SqlParameter("@trim",trim);
            var modelParm = new SqlParameter("@model", model);

            return await this.Database.SqlQuery<Car>
                ("CarsYearMakeModelTrim @year,@make,@model,@trim",yearParm,makeParm,modelParm,trimParm).ToListAsync();

        }

        // Get Cars with paging and search
        public async Task<List<Car>> GetSearchCar(string year, string make, string model, string trim, string filter, bool? paging, int page, int perpage, string sortcolumn, string sortdirection)
        {
            var yearParm = new SqlParameter("@year", year);
            var makeParm = new SqlParameter("@make", make ?? "");
            var modelParm = new SqlParameter("@model", model ?? "");
            var trimParm = new SqlParameter("@trim", trim ?? "");
            var filterParm = new SqlParameter("@filter", filter ?? "");
            var pagingParm = new SqlParameter("@paging", paging ?? false);
            var pageParm = new SqlParameter("@page", page);
            var perPageParm = new SqlParameter("@perpage", perpage);
            var sortcolumnParm = new SqlParameter("@sortcolumn",sortcolumn);
            var sortdirectionParm = new SqlParameter("@sortdirection", sortdirection);

            var filtered = await this.Database.SqlQuery<Car>
                ("GetSearchCar @year,@make,@model,@trim,@filter,@paging,@page,@perpage,@sortcolumn,@sortdirection",yearParm,makeParm,modelParm,trimParm,filterParm,pagingParm,pageParm,perPageParm,sortcolumnParm,sortdirectionParm).ToListAsync();
            return filtered;
        }



















    }
}