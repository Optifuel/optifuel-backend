using ApiCos.DTOs.CompanyDTO;
using ApiCos.DTOs.UserDTO;
using ApiCos.DTOs.VehicleDTO;
using ApiCos.ExceptionApi;
using ApiCos.Models.Entities;
using ApiCos.Response;
using ApiCos.Services.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiCos.Controllers
{
    public class CompanyController : GenericController<CompanyController>
    {
        public CompanyController(ILogger<CompanyController> logger, IUnitOfWork unitOfWork, IMapper mapper) : base(logger, unitOfWork, mapper)
        {
        }

        [HttpPost]
        [Route("api/[controller]/AddCompany")]
        public async Task<ActionResult<UserRequest>> AddCompany([FromBody] CompanyRequest company)
        {
            try
            {
                Company companyTable = _mapper.Map<Company>(company);
                await _unitOfWork.Company.Add(companyTable);
                await _unitOfWork.CompleteAsync();
                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, company)); 
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch (Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }

        }

        [HttpGet]
        [Route("api/[controller]/GetUsersByBusinessName")]
        public async Task<ActionResult<ICollection<User>?>> GetUsersByBusinessName([FromQuery] string businessName)
        {
            try
            {
                List<User>? usersDb = await _unitOfWork.Company.GetUsersByBusinessName(businessName);
                List<UserSending>? users = _mapper.Map<List<User>, List<UserSending>>(usersDb);

                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, users));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }
        }

        [HttpGet]
        [Route("api/[controller]/GetVehiclesByBusinessName")]
        public async Task<ActionResult<ICollection<User>?>> GetVehiclesByBusinessName([FromQuery] string businessName)
        {
            try
            {
                var vehiclesDb = await _unitOfWork.Company.GetVehiclesByBusinessName(businessName);
                var vehicles = _mapper.Map<List<Vehicle>, List<VehicleRequest>>(vehiclesDb);

                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, vehicles));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }
        }

        [HttpGet]
        [Route("api/[controller]/GetCompanyVatNumber")]
        public async Task<ActionResult<Company>> GetByVatNumber([FromQuery] string vatNumber)
        {
            try
            {
                var company = await _unitOfWork.Company.GetCompanyByVatNumber(vatNumber);
                var companySending = _mapper.Map<CompanySending>(company);
                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success,companySending ));
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
