using Microsoft.AspNetCore.Mvc;
using ApiCos.Models.Entities;
using ApiCos.Services.IRepositories;
using ApiCos.DTOs.UserDTO;
using ApiCos.Controllers;
using AutoMapper;
using ApiCos.Response;
using ApiCos.LoginAuthorization;
using ApiCos.ExceptionApi;

namespace ApiCOS.Controllers
{

    [ApiController]
    public class UserController : GenericController<UserController>
    {
        public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork, IMapper mapper) : base(logger, unitOfWork, mapper)
        {
        }

        [HttpGet]
        [Route("api/[controller]/GetUserByEmailAndPassword")]
        public async Task<ActionResult<UserSending>> GetUserByEmail([FromQuery] string email, [FromQuery] string Password)
        {
            try
            {
                User? user = await _unitOfWork.User.GetByEmailAndPassword(email, Password);
                UserSending userSending = _mapper.Map<UserSending>(user);
                userSending.token = LoginAuthorization.addAuthorization(userSending.Email);
                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, userSending));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }
        }

        [HttpGet]
        [Route("api/[controller]/checkAuthorization")]
        public async Task<ActionResult<string>> checkAuthorization([FromQuery]string email, [FromQuery] string token)
        {
            if(LoginAuthorization.checkAuthorization(email, token))
            {
                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, "Authorization"));
            }
            else
            {
                return BadRequest(ResponseHandler.GetApiResponse(ResponseType.Failure, "Failure"));
            }
        }

        [HttpPost]
        [Route("api/[controller]/AddUser")]
        public async Task<ActionResult<UserRequest>> AddUser([FromBody] UserRequest user)
        {
            try
            {
                await _unitOfWork.User.Add(_mapper.Map<User>(user), user.BusinessName, user.Password);
                await _unitOfWork.CompleteAsync();

                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, user));   
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }

        }

        [HttpPost]
        [Route("api/[controller]/EditUser")]
        public async Task<ActionResult<UserRequest>> editUser([FromBody] UserEdit user)
        {
            try
            {
                await _unitOfWork.User.EditUser(_mapper.Map<User>(user));
                await _unitOfWork.CompleteAsync();

                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, _mapper.Map<UserSending>(user)));
            } catch(BaseException e)
            {
                return BadRequest(ResponseHandler.GetApiResponse(e.id, e.description));
            } catch(Exception e)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(e));
            }
        }

        [HttpPut]
        [Route("api/[controller]/VerificationUser")]
        public async Task<ActionResult<string>> verificationUser([FromQuery] string email, [FromQuery] int token)
        {
            try
            {
                await _unitOfWork.User.ValidationUser(email, token);
                return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, "Success"));
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
