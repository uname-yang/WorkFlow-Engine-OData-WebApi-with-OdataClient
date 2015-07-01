using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using WebAPIwithODataSample.Extension.BasicAuth;
using WebAPIwithODataSample.Models.SampleModel;

namespace WebAPIwithODataSample.Controllers
{
    public class VediosController : ODataController
    {
        private VedioContext _db = new VedioContext();

        #region Built-in
        [BasicAuthorize]
        [EnableQuery(PageSize = 3)]
        public IHttpActionResult Get() //http://localhost:53520/odata/Vedios Get
        {
            return Ok(_db.Vedio);
        }
        public Vedio Get([FromODataUri] int key) //http://localhost:53520/odata/Vedios(1) Get
        {
            var v = _db.Vedio.FirstOrDefault(p => p.ID == key);
            return v;
        }
        public IHttpActionResult Post([FromBody]Vedio vedio) //http://localhost:53520/odata/Vedios Post:{"ID":2,"Title":"Inferno of Retribution","Year":2005}
        {
            _db.Vedio.Add(vedio);
            return Created(vedio);
        }
        public IHttpActionResult Delete([FromODataUri] int key) //http://localhost:53520/odata/Vedios(1) Delete
        {
            var movie = _db.Vedio.FirstOrDefault(m => m.ID == key);
            if (movie == null)
            {
                return BadRequest(ModelState);
            }
            _db.Vedio.Remove(movie);
            return Ok(movie);
        }
        public IHttpActionResult Put([FromODataUri] int key, Vedio vedio)
        {
            return Ok();
        }//http://localhost:53520/odata/Vedios(1) Put:{"ID":2,"Title":"Inferno of Retribution","Year":2005}
        #endregion

        #region Function
        [HttpGet]
        [ODataRoute("CheckOut(key={key})")]
        public Vedio CheckOut([FromODataUri]int key)        // http://localhost:53520/odata/CheckOut(key=4) Get
        {
            var movie = _db.Vedio.FirstOrDefault(m => m.ID == key);
            if (movie == null)
            {
                return null;
            }

            if (!TryCheckoutMovie(movie))
            {
                return null;
            }

            return movie;
        }

        #endregion

        #region Action
        [HttpPost]
        [ODataRoute("ListAc")]
        public IHttpActionResult ListAc(ODataActionParameters parameters)
        {
            Vedio v = _db.Vedio.First();
            return Ok(23);
        }

        [HttpPost]
        [ODataRoute("ReturnAc")]
        public IHttpActionResult ReturnAc(ODataActionParameters parameters)  //http://localhost:53520/odata/ReturnAc {"key":1}
        {
            int key = parameters["key"] == null ? 0 : (int)parameters["key"];
            var movie = _db.Vedio.FirstOrDefault(m => m.ID == key);
            if (movie == null)
            {
                return BadRequest(ModelState);
            }

            movie.DueDate = null;

            return Ok(movie);
        }

        [HttpPost]
        [ODataRoute("CreateVedio")]
        public IHttpActionResult CreateVedioAc(ODataActionParameters parameters) //http://localhost:53520/odata/CreateVedio {"vedio":{"ID":2,"Title":"Inferno of Retribution","Year":2005}}
        {
            Vedio vedio = parameters["vedio"] as Vedio;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _db.Vedio.Add(vedio);
            return Created(vedio);
            //return Created("sss");
        }


        [HttpPost]
        [ODataRoute("CheckOutMany")]
        public IHttpActionResult CheckOutMany(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Client passes a list of movie IDs to check out.
            var movieIDs = new HashSet<int>(parameters["MovieIDs"] as IEnumerable<int>);

            // Try to check out each movie in the list.
            var results = new List<Vedio>();
            foreach (Vedio movie in _db.Vedio.Where(m => movieIDs.Contains(m.ID)))
            {
                if (TryCheckoutMovie(movie))
                {
                    results.Add(movie);
                }
            }

            // Return a list of the movies that were checked out.
            return Ok(results);
        } //http://localhost:53520/odata/CheckOutMany {"MovieIDs":[1,2,3]}

        #endregion

        #region Private
        protected Vedio GetMovieByKey(int key)
        {
            return _db.Vedio.FirstOrDefault(m => m.ID == key);
        }

        private bool TryCheckoutMovie(Vedio movie)
        {
            if (movie.IsCheckedOut)
            {
                return false;
            }
            else
            {
                // To check out a movie, set the due date.
                movie.DueDate = DateTime.Now.AddDays(7);
                return true;
            }
        }
        #endregion
    }
}