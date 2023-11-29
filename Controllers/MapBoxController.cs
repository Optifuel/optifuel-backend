using ApiCos.ExceptionApi;
using ApiCos.Models.Entities;
using ApiCos.Response;
using ApiCos.Services.IRepositories;
using Microsoft.AspNetCore.Mvc;


namespace ApiCos.Controllers
{
    [ApiController]
    public class MapBoxController : ControllerBase
    {
        private readonly IMapBox _mapBox;

        public MapBoxController(IMapBox mapBox)
        {
            _mapBox = mapBox;
        }

        [HttpGet]
        [Route("api/[controller]/GetCoordinates")]
        public async Task<ActionResult<Coordinates>> GetCoordinates(string city)
        {
            try
            {
                Coordinates coordinates = await _mapBox.GetCoordinates(city);

                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, coordinates));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }
        }

        [HttpGet]
        [Route("api/[controller]/GetPathByTown")]
        public async Task<ActionResult<List<double[]>>> GetPathByTown(string startTown, string endTown)
        {
            try
            {
                var result = await _mapBox.GetPathByTown(startTown, endTown);

                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, result));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }
        }
    }
}
