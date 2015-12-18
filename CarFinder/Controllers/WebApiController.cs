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
    /// <summary>
    /// The WebApi Controller
    /// </summary>
    [RoutePrefix("api/WebApi")]
    public class WebApiController : ApiController
    {   
        private ApplicationDbContext db = new ApplicationDbContext();


        /// <summary>
        /// class created to pass Id as parameter to getCar()
        /// </summary>
        public class IdParm
        {
            /// <summary>
            /// id property for the IdParm Class
            /// </summary>
            public int id { get; set; }
        }
        /// <summary>
        /// Parameters for GetSearchCars
        /// </summary>
        public class ControllerParams
        {
            /// <summary>
            /// Search year for the car
            /// </summary>
            public string year { get; set; }
            /// <summary>
            /// Make of the car to be searched for
            /// </summary>
            public string make { get; set; }
            /// <summary>
            /// Model of the car to be searched for
            /// </summary>
            public string model { get; set; }
            /// <summary>
            /// Trim of the car to be saerched for
            /// </summary>
            public string trim { get; set; }
            /// <summary>
            /// Special search string
            /// </summary>
            public string filter { get; set; }
            /// <summary>
            /// Paging enabled or not
            /// </summary>
            public bool? paging { get; set; }
            /// <summary>
            /// Page no
            /// </summary>
            public int? page { get; set; }
            /// <summary>
            /// How many records to be displayed per page
            /// </summary>
            public int? perpage { get; set; }
            /// <summary>
            /// Column to sort with
            /// </summary>
            public string sortcolumn { get; set; }
            /// <summary>
            /// Sort direction Ascending or Descending
            /// </summary>
            public string sortdirection { get; set; }
            /// <summary>
            /// Reverse Sort
            /// </summary>
            public bool? sortByReverse { get; set; }
            
        }

        // Get AllYears from the cars table
        /// <summary>
        /// This action calls a stored procedure AllYears which gets all the years listed in the cars database.
        /// </summary>
        /// <returns>List of all the years of datatype string.</returns>
        [HttpPost]
        [Route("AllYears")]
        public async Task<IHttpActionResult> AllYears()
        {
            return Ok(await db.AllYears());
        }

        // Get Makes By Year
        /// <summary>
        /// This action calls a stored procedure MakesByYear and gets all the Makes for the required year.
        /// </summary>
        /// <param name="selected">An object with select parameter Year</param>
        /// <returns>A list of all makes of datatype string based on the input parameters.</returns>
        [HttpPost]
        [Route("MakesByYear")]
        public async Task<IHttpActionResult> MakesByYear(ControllerParams selected)
        {
            return Ok(await db.MakesByYear(selected.year));
        }

        // Get Models By Year and Make
        /// <summary>
        /// This action calls a stored procedure ModelsByYearMake and gets all the Models for the required Year and Make.
        /// </summary>
        /// <param name="selected">An object with select parameters Year and Make</param>
        /// <returns>A list of all the models of datatype string based on the input parameters.</returns>
        [HttpPost]
        [Route("ModelsByYearMake")]
        public async Task<IHttpActionResult> ModelsByYearMake(ControllerParams selected)
        {
            return Ok(await db.ModelsByYearMake(selected.year,selected.make));
        }

        // Get Trims by Year, Make and Model
        /// <summary>
        /// This action calls a stored procedure Trims and gets all the Trims for the required Year, Make and Model.
        /// </summary>
        /// <param name="selected">An object with selected parameters Year, Make and Model</param>
        /// <returns>A list of trims by Year, Make and Model</returns>
        [HttpPost]
        [Route("Trims")]
        public async Task<IHttpActionResult> Trims(ControllerParams selected)
        {
            return Ok(await db.Trims(selected.year, selected.make, selected.model));
        }

        // Get Cars By Year
        /// <summary>
        /// This action calls a stored procedure CarsByYear.
        /// </summary>
        /// <param name="selected">An object with selected parameters Year</param>
        /// <returns>A list of all the cars based on the input parameter year.</returns>
        [HttpPost]
        [Route("CarsByYear")]
        public async Task<IHttpActionResult> CarsByYear(ControllerParams selected)
        {
            return Ok(await db.CarsByYear(selected.year));
        }

        // Get Cars by Year and Make
        /// <summary>
        /// This action calls a stored procedure CarsByYearMake and gets all the Cars for the required Year.
        /// </summary>
        /// <param name="selected">An object with selected parameters Year and Make</param>
        /// <returns>A list of cars based on the input parameters year and make.</returns>
        [HttpPost]
        [Route("CarsByYearMake")]
        public async Task<IHttpActionResult> CarsByYearMake(ControllerParams selected)
        {
            return Ok(await db.CarsByYearMake(selected.year,selected.make));
        }

        // Get Cars by Year, Make and Model
        /// <summary>
        /// This action calls a stored procedure CarsYearMakeModel and returns all the Cars for the required Year, Make and Model.
        /// </summary>
        /// <param name="selected">An object with selected parameters Year, Make and Model</param>
        /// <returns>A list of cars based on the input parameters year, make and model.</returns>
        [HttpPost]
        [Route("CarsYearMakeModel")]
        public async Task<IHttpActionResult> CarsYearMakeModel(ControllerParams selected)
        {
            return Ok(await db.CarsYearMakeModel(selected.year, selected.make, selected.model));
        }

        // Get Cars by Year, Make, Model and Trim
        /// <summary>
        /// This action calls a stored procedure CarsYearMakeModelTrim and returns all the Cars for the required Year, Make, Model and Trim.
        /// </summary>
        /// <param name="selected">An object with selected parameters Year, Make, Model and Trim</param>
        /// <returns>A list of cars based on the input parameters year, make, model and trim.</returns>
        [HttpPost]
        [Route("CarsYearMakeModelTrim")]
        public async Task<IHttpActionResult> CarsYearMakeModelTrim(ControllerParams selected)
        {
            return Ok(await db.CarsYearMakeModelTrim(selected.year,selected.make,selected.model,selected.trim));
        }

        
        
        /// <summary>
        /// This action calls a stored procedure GetSearchCar. Some of the parameters are optional. 
        /// SORTING - It also sorts the data based on the column name provided in the imput (year,make, model_name,model_trim). 
        /// If the column name for sort is not provided then the data will be sorted by ID.
        /// PAGING - If paging is on, the number of records displayed will depend on the input parameter perpage.
        ///             If paging is off, all the records will be displayed at once.
        /// </summary>
        /// <param name="selected">Object of class ControllerParm</param>
        /// <returns>A list of cars</returns>
        [HttpPost]
        [Route("GetSearchCar")]
        public async Task<IHttpActionResult> GetSearchCar(ControllerParams selected)
        {
            return Ok(await db.GetSearchCar(selected.year,selected.make,selected.model,selected.trim,selected.filter,selected.paging,selected.page,selected.perpage,
                selected.sortcolumn,selected.sortdirection,selected.sortByReverse));
        }

        /// <summary>
        /// Gets the count of selected cars for server side data tables
        /// </summary>
        /// <param name="selected">An object with selected parameters Year, Make, Model, Trim and filter</param>
        /// <returns>Car Count</returns>
        [HttpPost]
        [Route("GetCarCount")]
        public async Task<IHttpActionResult> GetCarCount(ControllerParams selected)
        {
            return Ok(await db.GetCarCount(selected.year, selected.make, selected.model, selected.trim, selected.filter));
        }

        /// <summary>
        /// This action calls a stored procedure GetCar and returns all the information for a particular Car.
        /// This information includes the car image (if any). It also gets the recall information (if any) listed with NHTSA.
        /// </summary>
        /// <param name="id">Id of the car for which information is required</param>
        /// <returns>Image and recall information (if any) for the car specified by the id.</returns>
        [HttpPost]
        [Route("GetCar")]
        public async Task<IHttpActionResult> GetCar(IdParm id)
        {
            // Getting car images through Bing Search 
            var car = db.Cars.Find(id.id);
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
