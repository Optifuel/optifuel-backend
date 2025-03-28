﻿using System;
using Api.ExceptionApi;
using Api.Models.Entities;
using Api.Response;
using Api.Services.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Api.DTOs.MapBox;


namespace Api.Controllers
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
        public async Task<ActionResult<List<double[]>>> GetPathByTown(double initLongitude, double initLatitude, double endLongitude, double endLatitude)
        {
            try
            {
                var result = await _mapBox.GetPathByTown(initLongitude, initLatitude, endLongitude, endLatitude);

                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, result));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }
        }

        [HttpPost]
        [Route("api/[controller]/FindGasStation")]
        //double percentTank, double initLongitude, double initLatitude, double endLongitude, double endLatitude
        public async Task<ActionResult<FindGasStationSending>> FindGasStation([FromQuery] string licensePlate,[FromQuery] double percentTank,[FromBody] List<Coordinates> listPoints)
        {
            try
            {
                Vehicle vehicle = await _unitOfWork.Vehicle.GetByLicensePlate(licensePlate);
                var (list, waypoint) = await _mapBox.FindGasStation(vehicle, percentTank, listPoints);
                Console.WriteLine("[DEBUG CONTROLLER] list "+ list.Count);
                Console.WriteLine("[DEBUG CONTROLLER] waypoint "+ waypoint.Count);


                List<GasStationSending> temp = new List<GasStationSending>();
                foreach(var item in list)
                {
                    temp.Add(new GasStationSending
                    {
                        name = item.GasStationName,
                        coordinates = new Coordinates
                        {
                            Latitude = (double)item.Latitude,
                            Longitude = (double)item.Longitude
                        },
                        price = item.GasStationPrices.FirstOrDefault().Price,
                        type = vehicle.FuelType,
                        address = item.Address
                    });
                    ;
                }

                FindGasStationSending listToSend = new FindGasStationSending
                {
                    Station = temp,
                    coordinates = waypoint
                };

                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, listToSend));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            }
        }

    }
}
