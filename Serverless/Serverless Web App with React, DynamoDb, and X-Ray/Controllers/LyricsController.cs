using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWSAppService;
using AWSAppService.Auth;
using AWSAppService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Musiquizza_React.Models;

namespace Musiquizza_React.Controllers
{
    [Produces("application/json")]
    [Route("api/Lyrics")]
    public class LyricsController : Controller
    {
        public static Song SongReturned;
        private readonly DBDataService<IData> _dbDataService;

        public LyricsController(IAWSAppService<IData> dataService)
        {
            _dbDataService = (DBDataService<IData>)dataService;
        }

        [HttpGet]
        public async Task<string> Get()
        {

            //choose a random number and pull song on that id
            Random r = new Random();
            int rInt = r.Next(0, 64); //for ints

            Song s = await _dbDataService.RetrieveDataFromDB<Song>(rInt);

            SongReturned = s;
            await SongReturned.GetLyrics();

            return SongReturned.SongLyric;
        }

        [HttpPost]
        public JsonResult Post([FromBody]QuizAnswer answer)
        {
            //if artist and title are the same as posted, return true
            if (answer.Artist.Equals(SongReturned.Artist, StringComparison.CurrentCultureIgnoreCase) && answer.Title.Equals(SongReturned.Title, StringComparison.CurrentCultureIgnoreCase))
            {
                return Json(new { isCorrect = true });
            }
            else
            {
                return Json(new { isCorrect = false });
            }

        }


    }
}
