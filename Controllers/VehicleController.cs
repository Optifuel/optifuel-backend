using Api.DTOs.UserDTO;
using Api.DTOs.VehicleDTO;
using Api.ExceptionApi;
using Api.Models.Entities;
using Api.Response;
using Api.Services.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Api.LoginAuthorization;

namespace Api.Controllers
{
    [ApiController]
    public class VehicleController : GenericController<VehicleController>
    {
        public VehicleController(ILogger<VehicleController> logger, IUnitOfWork unitOfWork, IMapper mapper) : base(logger, unitOfWork, mapper)
        {
        }

        [HttpPost]
        [Route("api/[controller]/AddVehicle")]
        public async Task<ActionResult<Vehicle>> AddVehicle([FromBody] VehicleRequest data)
        {
            try
            {
                ResponseType responseType = ResponseType.Success;
                Vehicle vehicle = _mapper.Map<Vehicle>(data);
                Company company = await _unitOfWork.Company.GetCompanyByBusinessName(data.BusinessName);
                User user = await _unitOfWork.User.GetByEmail(data.email);
                await _unitOfWork.Vehicle.Add(vehicle, company, user);
                await _unitOfWork.CompleteAsync();

                return Ok(ResponseHandler.GetApiResponse(responseType, data));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }

        }

        [HttpPost]
        [Route("api/[controller]/GetVehicleByLicensePlate")]
        public async Task<ActionResult<Vehicle>> GetVehicleByLicensePlate([FromQuery] string licensePlate)
        {
            try
            {
                ResponseType responseType = ResponseType.Success;
                Vehicle? vehicle = await _unitOfWork.Vehicle.GetByLicensePlate(licensePlate);
                VehicleSending vehicleResponse = _mapper.Map<VehicleSending>(vehicle);
                return Ok(ResponseHandler.GetApiResponse(responseType, vehicleResponse));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }
        }

        [HttpGet]
        [Route("api/[controller]/GetListVehicleByBrand")]
        public async Task<ActionResult<List<Vehicle>>> GetListVehicleByBrand([FromQuery] string businessName, [FromQuery] string brand)
        {
            try
            {
                ResponseType responseType = ResponseType.Success;
                List<Vehicle?>? vehicles = await _unitOfWork.Vehicle.SearchByBrand(businessName, brand);
                List<VehicleRequest>? vehiclesResponse = _mapper.Map<List<Vehicle>,List<VehicleRequest>>(vehicles);
                return Ok(ResponseHandler.GetApiResponse(responseType, vehiclesResponse));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }
        }

        [HttpGet]
        [Route("api/[controller]/GetListVehicleByModel")]
        public async Task<ActionResult<List<Vehicle>>> GetListVehicleByModel([FromQuery] string businessName, [FromQuery] string model)
        {
            try
            {
                ResponseType responseType = ResponseType.Success;
                List<Vehicle?>? vehicles = await _unitOfWork.Vehicle.SearchByModel(businessName, model);
                List<VehicleRequest>? vehiclesResponse = _mapper.Map<List<Vehicle>, List<VehicleRequest>>(vehicles);
                return Ok(ResponseHandler.GetApiResponse(responseType, vehiclesResponse));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }
        }

        [HttpPut]
        [Route("api/[controller]/EditVehicle")]
        public async Task<ActionResult<Vehicle>> EditVehicle([FromBody] VehicleRequest data)
        {
            try
            {
                ResponseType responseType = ResponseType.Success;
                Vehicle vehicle = _mapper.Map<Vehicle>(data);
                await _unitOfWork.Vehicle.EditVehicle(vehicle, data.BusinessName);
                await _unitOfWork.CompleteAsync();

                return Ok(ResponseHandler.GetApiResponse(responseType, data));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }
        }

        [HttpDelete]
        [Route("api/[controller]/DeleteVehicle")]
        public async Task<ActionResult<Vehicle>> DeleteVehicle(string email, string token ,[FromQuery] string licensePlate)
        {
            try
            {
                if(!Api.LoginAuthorization.LoginAuthorization.checkAuthorization(email, token))
                    return BadRequest(ResponseHandler.GetApiResponse(ResponseType.Failure, "Unauthorized"));

                await _unitOfWork.Vehicle.Delete(licensePlate);
                await _unitOfWork.CompleteAsync();

                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, licensePlate));
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
