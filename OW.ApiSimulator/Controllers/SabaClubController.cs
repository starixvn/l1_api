using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OW.ApiSimulator.Models.OW;

namespace OW.ApiSimulator.Controllers
{
    public class SabaClubController : Controller
    {
        private readonly IServiceProvider serviceProvider;

        public SabaClubController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UpdateEnterPortalResult()
        {
            return View(serviceProvider.GetRequiredService<EnterPortalResult>());
        }

        [HttpPost]
        public IActionResult UpdateEnterPortalResult(EnterPortalResult model)
        {
            var result = serviceProvider.GetRequiredService<EnterPortalResult>();
            result.ErrorCode = model.ErrorCode;
            result.ErrorDescription = model.ErrorDescription;

            return View(model);
        }

        [HttpPost]
        public IActionResult EnterPortal([FromBody] OWRequest<EnterPortalCall> request)
        {
            var result = serviceProvider.GetRequiredService<EnterPortalResult>();
            result.Seq = request.Call?.Seq;

            return Ok(new OWResponse<EnterPortalResult>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Result = result
            });
        }

        [HttpGet]
        public IActionResult UpdateGetBalanceResult()
        {
            return View(serviceProvider.GetRequiredService<GetBalanceResult>());
        }

        [HttpPost]
        public IActionResult UpdateGetBalanceResult(GetBalanceResult model)
        {
            var result = serviceProvider.GetRequiredService<GetBalanceResult>();
            result.ErrorCode = model.ErrorCode;
            result.ErrorDescription = model.ErrorDescription;
            result.Balance = model.Balance;

            return View(model);
        }

        [HttpPost]
        public IActionResult Balance([FromBody] OWRequest<GetBalanceCall> request)
        {
            var result = serviceProvider.GetRequiredService<GetBalanceResult>();
            result.Seq = request.Call?.Seq;

            return Ok(new OWResponse<GetBalanceResult>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Result = result
            });
        }

        [HttpGet]
        public IActionResult UpdatePlaceBetResult()
        {
            return View(serviceProvider.GetRequiredService<PlaceBetResult>());
        }

        [HttpPost]
        public IActionResult UpdatePlaceBetResult(PlaceBetResult model)
        {
            var result = serviceProvider.GetRequiredService<PlaceBetResult>();
            result.ErrorCode = model.ErrorCode;
            result.ErrorDescription = model.ErrorDescription;
            result.ObTransId = model.ObTransId;

            return View(model);
        }

        [HttpPost]
        public IActionResult PlaceBet([FromBody] OWRequest<PlaceBetCall> request)
        {
            var result = serviceProvider.GetRequiredService<PlaceBetResult>();
            result.Seq = request.Call?.Seq;

            if (result.ErrorCode == 0)
            {
                result.ObCustId = request.Call?.ObCustId;
                var balanceResult = serviceProvider.GetRequiredService<GetBalanceResult>();
                result.Balance = balanceResult.Balance = balanceResult.Balance - request.Call.Amount;
            }

            return Ok(new OWResponse<PlaceBetResult>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Result = result
            });
        }

        [HttpGet]
        public IActionResult UpdateEndGameResult()
        {
            return View(serviceProvider.GetRequiredService<EndGameResult>());
        }

        [HttpPost]
        public IActionResult UpdateEndGameResult(EndGameResult model)
        {
            var result = serviceProvider.GetRequiredService<EndGameResult>();
            result.ErrorCode = model.ErrorCode;
            result.ErrorDescription = model.ErrorDescription;

            return View(model);
        }

        [HttpPost]
        public IActionResult EndGame([FromBody] OWRequest<EndGameCall> request)
        {
            var result = serviceProvider.GetRequiredService<EndGameResult>();

            result.Seq = request.Call?.Seq;

            if (result.ErrorCode == 0)
            {
                result.ObCustId = request.Call?.ObCustId;
                var balanceResult = serviceProvider.GetRequiredService<GetBalanceResult>();
                balanceResult.Balance += request.Call.TotalWin;
            }

            return Ok(new OWResponse<EndGameResult>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Result = result
            });
        }

        [HttpGet]
        public IActionResult UpdateVoidGameResult()
        {
            return View(serviceProvider.GetRequiredService<VoidGameResult>());
        }

        [HttpPost]
        public IActionResult UpdateVoidGameResult(VoidGameResult model)
        {
            var result = serviceProvider.GetRequiredService<VoidGameResult>();
            result.ErrorCode = model.ErrorCode;
            result.ErrorDescription = model.ErrorDescription;

            return View(model);
        }

        [HttpPost]
        public IActionResult VoidGame([FromBody] OWRequest<VoidGameCall> request)
        {
            var result = serviceProvider.GetRequiredService<VoidGameResult>();
            result.Seq = request.Call?.Seq;
            if (result.ErrorCode == 0)
            {
                result.ObCustId = request.Call?.ObCustId;
            }

            return Ok(new OWResponse<VoidGameResult>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Result = result
            });
        }

        [HttpGet]
        public IActionResult UpdateCheckMaintenanceResult()
        {
            return View(serviceProvider.GetRequiredService<CheckMaintenanceResult>());
        }

        [HttpPost]
        public IActionResult UpdateCheckMaintenanceResult(CheckMaintenanceResult model)
        {
            var result = serviceProvider.GetRequiredService<CheckMaintenanceResult>();
            result.ErrorCode = model.ErrorCode;
            result.ErrorDescription = model.ErrorDescription;
            result.StartTime = model.StartTime;
            result.EndTime = model.EndTime;

            return View(model);
        }

        [HttpPost]
        public IActionResult CheckMaintenance([FromBody] OWRequest<CheckMaintenanceCall> request)
        {
            var result = serviceProvider.GetRequiredService<CheckMaintenanceResult>();
            result.Seq = request.Call?.Seq;
       
            return Ok(new OWResponse<CheckMaintenanceResult>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Result = result
            });
        }
    }
}