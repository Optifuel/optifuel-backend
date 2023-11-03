using ApiCos.Response;
using ApiCos.Services;
using ApiCos.Services.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiCos.Controllers
{
    [ApiController]
    public class GasStationController : GenericController<GasStationController>
    {
        private readonly IGasStation _gasStation;

        public GasStationController(ILogger<GasStationController> logger, IUnitOfWork unitOfWork, IMapper mapper, IGasStation gasStation) : base(logger, unitOfWork, mapper)
        {
            _gasStation = gasStation;
        }

        [HttpGet]
        [Route("api/[controller]/GetUserByEmail")]
        public async Task<ActionResult<string>> UpdateGasStation([FromQuery] string email)
        {
            ResponseType responseType = ResponseType.Success;
            string? gas = await _gasStation.UpdateGasStation();
            return Ok(ResponseHandler.GetApiResponse(responseType, false));

        }
    }
}
