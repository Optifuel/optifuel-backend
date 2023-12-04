using ApiCos.ExceptionApi;
using ApiCos.Models.Entities;
using ApiCos.Response;
using ApiCos.Services.IRepositories;
using Microsoft.AspNetCore.Mvc;
using ApiCos.DTOs.MapBox;


namespace ApiCos.Controllers
{
    [ApiController]
    public class MapBoxController : ControllerBase
    {
        private readonly IMapBox _mapBox;
        protected readonly IUnitOfWork _unitOfWork;


        public MapBoxController(IMapBox mapBox, IUnitOfWork unitOfWork)
        {
            _mapBox = mapBox;
            _unitOfWork = unitOfWork;
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

        [HttpGet]
        [Route("api/[controller]/FindGasStation")]
        public async Task<ActionResult<List<GasStationSending>>> FindGasStation(string licensePlate, string startTown, string endTown)
        {
            try
            {
                Vehicle vehicle = await _unitOfWork.Vehicle.GetByLicensePlate(licensePlate);
                var list = await _mapBox.FindGasStation(vehicle, startTown, endTown);

                List<GasStationSending> listToSend = new List<GasStationSending>();
                foreach(var item in list)
                {
                    listToSend.Add(new GasStationSending
                    {
                        name = item.GasStationName,
                        coordinates = new Coordinates
                        {
                            Latitude = (double)item.Latitude,
                            Longitude = (double)item.Longitude
                        },
                        price = item.GasStationPrices.Where(x => x.FuelType == vehicle.FuelType).FirstOrDefault().Price,
                        type = vehicle.FuelType,
                        address = item.Address
                    });
                    ;
                }

                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, listToSend));
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
