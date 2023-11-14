using Microsoft.AspNetCore.Mvc;
using ApiCos.Models.Entities;
using ApiCos.Services.IRepositories;
using ApiCos.DTOs.UserDTO;
using ApiCos.Controllers;
using AutoMapper;
using ApiCos.Response;
using ApiCos.DTOs.Common;

namespace ApiCOS.Controllers
{

    [ApiController]
    public class UserController : GenericController<UserController>
    {
        public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork, IMapper mapper) : base(logger, unitOfWork, mapper)
        {
        }

        [HttpGet]
        [Route("api/[controller]/GetUserByEmail")]
        public async Task<ActionResult<UserSending>> GetUserByEmail([FromQuery] string email)
        {

            ResponseType responseType = ResponseType.Success;
            User user = await _unitOfWork.User.GetByEmail(email);
            UserSending userSending = _mapper.Map<UserSending>(user);
            if(user == null)
                responseType = ResponseType.NotFound;
            return Ok(ResponseHandler.GetApiResponse(responseType, userSending));

        }

        [HttpGet]
        [Route("api/[controller]/GetUserByEmailAndPassword")]
        public async Task<ActionResult<UserSending>> GetUserByEmail([FromQuery] string email, [FromQuery] string Password)
        {
            ResponseType responseType = ResponseType.Success;
            try
            {
                User user = await _unitOfWork.User.GetByEmailAndPassword(email, Password);
                UserSending userSending = _mapper.Map<UserSending>(user);
                return Ok(ResponseHandler.GetApiResponse(responseType, userSending));
            } catch(Exception e)
            {
                return Ok(ResponseHandler.GetApiResponse(ResponseType.Failure, e.Message));
            }
        }

        [HttpPost]
        [Route("api/[controller]/AddUser/")]
        public async Task<ActionResult<UserRequest>> AddUser([FromBody] UserRequest user)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    await _unitOfWork.User.Add(_mapper.Map<User>(user), user.BusinessName, user.Password);
                    await _unitOfWork.CompleteAsync();

                    return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, user));
                }
                return Ok(ResponseHandler.GetApiResponse(ResponseType.Failure, "Error"));
            } catch(Exception e)
            {
                return Ok(ResponseHandler.GetApiResponse(ResponseType.Failure, e.Message));
            }

        }

        [HttpPost]
        [Route("api/[controller]/EditUser/")]
        public async Task<ActionResult<UserRequest>> editUser([FromBody] UserSending user)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    await _unitOfWork.User.EditUser(_mapper.Map<User>(user));
                    await _unitOfWork.CompleteAsync();

                    return Ok(ResponseHandler.GetApiResponse(ResponseType.Success, _mapper.Map<UserSending>(user)));
                }
                return Ok(ResponseHandler.GetApiResponse(ResponseType.Failure, "Error"));
            } catch(Exception e)
            {
                return Ok(ResponseHandler.GetApiResponse(ResponseType.Failure, e.Message));
            }
        }

    }


}
