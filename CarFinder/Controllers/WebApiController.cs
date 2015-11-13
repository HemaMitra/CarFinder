using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using CarFinder.Models;
using CarFinder.Providers;
using CarFinder.Results;
using System.Threading.Tasks;
using Bing;
using Newtonsoft.Json;
using System.Configuration;   

namespace CarFinder.Controllers
{
    [RoutePrefix("api/WebApi")]
    public class WebApiController : ApiController
    {   
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // Get AllYears from the cars table
        /// <summary>
        /// This action calls a stored procedure AllYears.
        /// </summary>
        /// <returns>List of all the years of datatype string.</returns>
        [HttpGet]
        [Route("AllYears")]
        public async Task<IHttpActionResult> AllYears()
        {
            return Ok(await db.AllYears());
        }

        // Get Makes By Year
        /// <summary>
        /// This action calls a stored procedure MakesByYear.
        /// </summary>
        /// <param name="year"></param>
        /// <returns>A list of all makes of datatype string based on the input parameters.</returns>
        [HttpGet]
        [Route("MakesByYear")]
        public async Task<IHttpActionResult> MakesByYear(string year)
        {
            return Ok(await db.MakesByYear(year));
        }

        // Get Models By Year and Make
        /// <summary>
        /// This action calls a stored procedure ModelsByYearMake.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <returns>A list of all the models of datatype string based on the input parameters.</returns>
        [HttpGet]
        [Route("ModelsByYearMake")]
        public async Task<IHttpActionResult> ModelsByYearMake(string year, string make)
        {
            return Ok(await db.ModelsByYearMake(year,make));
        }

        // Get Trims by Year, Make and Model
        /// <summary>
        /// This action calls a stored procedure Trims.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <returns>A list of all the trims of datatype string based on the input parameters.</returns>
        [HttpGet]
        [Route("Trims")]
        public async Task<IHttpActionResult> Trims(string year, string make, string model)
        {
            return Ok(await db.Trims(year, make, model));
        }

        // Get Cars By Year
        /// <summary>
        /// This action calls a stored procedure CarsByYear.
        /// </summary>
        /// <param name="year"></param>
        /// <returns>A list of all the cars based on the input parameter year.</returns>
        [HttpGet]
        [Route("CarsByYear")]
        public async Task<IHttpActionResult> CarsByYear(string year)
        {
            return Ok(await db.CarsByYear(year));
        }

        // Get Cars by Year and Make
        /// <summary>
        /// This action calls a stored procedure CarsByYearMake.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <returns>A list of cars based on the input parameters year and make.</returns>
        [HttpGet]
        [Route("CarsByYearMake")]
        public async Task<IHttpActionResult> CarsByYearMake(string year, string make)
        {
            return Ok(await db.CarsByYearMake(year,make));
        }

        // Get Cars by Year, Make and Model
        /// <summary>
        /// This action calls a stored procedure CarsYearMakeModel.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <returns>A list of cars based on the input parameters year, make and model.</returns>
        [HttpGet]
        [Route("CarsYearMakeModel")]
        public async Task<IHttpActionResult> CarsYearMakeModel(string year, string make, string model)
        {
            return Ok(await db.CarsYearMakeModel(year, make, model));
        }

        // Get Cars by Year, Make, Model and Trim
        /// <summary>
        /// This action calls a stored procedure CarsYearMakeModelTrim.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <param name="trim"></param>
        /// <returns>A list of cars based on the input parameters year, make, model and trim.</returns>
        [HttpGet]
        [Route("CarsYearMakeModelTrim")]
        public async Task<IHttpActionResult> CarsYearMakeModelTrim(string year, string make, string model, string trim)
        {
            return Ok(await db.CarsYearMakeModelTrim(year,make,model,trim));
        }

        /// <summary>
        /// This action calls a stored procedure GetSearchCar. Some of the parameters are optional. 
        /// SORTING - It also sorts the data based on the column name provided in the imput (year,make, model_name,model_trim). 
        /// If the column name for sort is not provided then the data will be sorted by ID.
        /// PAGING - If paging is on, the number of records displayed will depend on the input parameter perpage.
        ///             If paging is off, all the records will be displayed at once.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <param name="trim"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <param name="page"></param>
        /// <param name="perpage"></param>
        /// <param name="sortcolumn"></param>
        /// <param name="sortdirection"></param>
        /// <returns>A list of cars based on the input parameters</returns>
        [HttpGet]
        [Route("GetSearchCar")]
        public async Task<IHttpActionResult> GetSearchCar(string year, string make, string model, string trim, string filter, 
            bool? paging, int page, int perpage, string sortcolumn,string sortdirection)
        {
            return Ok(await db.GetSearchCar(year,make,model,trim,filter,paging,page,perpage,sortcolumn,sortdirection));
        }

        /// <summary>
        /// This action calls a stored procedure GetCar.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Image and recall information (if any) for the car specified by the id.</returns>
        [Route("GetCar")]
        public async Task<IHttpActionResult> GetCar(int id)
        {
            // Getting car images through Bing Search 
            var car = db.Cars.Find(id);
            if (car == null)
                return await Task.FromResult(NotFound());

            HttpResponseMessage response;
            // Get Credentials for Bing from Web Config File
            string searchApiServiceRootUri = ConfigurationManager.AppSettings["searchApiServiceRootUri"];
            string accountKeyPass = ConfigurationManager.AppSettings["searchApiAccountKey"];

            var client = new BingSearchContainer(new Uri(searchApiServiceRootUri));
            client.Credentials = new NetworkCredential("accountKey", accountKeyPass);
            
            var marketData = client.Composite(
                "image",
                car.model_year + ' ' + car.make + ' ' + car.model_name + ' ' + car.model_trim,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
                ).Execute();

            var result = marketData.FirstOrDefault();
            var image = result != null ? result.Image : null;
            var firstImage = image != null ? image.FirstOrDefault() : null;
            var imgUrl = firstImage != null ? firstImage.MediaUrl : null;

            // Getting recalls from NHTSA
            dynamic recalls;
            string recallBaseAddress = ConfigurationManager.AppSettings["recallBaseAddress"];
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(recallBaseAddress);

                try
                {
                    response = await httpClient.GetAsync("webapi/api/Recalls/vehicle/modelyear/"+car.model_year+"/make/"
                        +car.make+"/model/"+car.model_name +"? FormatException=json");
                    
                    // JSON returns a string so deserialize it before sending it
                    recalls = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }

            return Ok(new { car, imgUrl, recalls });
        }
    }
}
