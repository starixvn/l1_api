
namespace OW.ApiSimulator.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using OW.ApiSimulator.Models.OW;

    public class CustomerController : Controller
    {
        private readonly IServiceProvider serviceProvider;

        public CustomerController(IServiceProvider serviceProvider)
        {

            this.serviceProvider = serviceProvider;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UpdateNotifyCustomerLoginResult()
        {
            return View(serviceProvider.GetRequiredService<NotfityCustomerLoginResult>());
        }

        [HttpPost]
        public IActionResult UpdateNotifyCustomerLoginResult(NotfityCustomerLoginResult model)
        {
            var result = serviceProvider.GetRequiredService<NotfityCustomerLoginResult>();
            result.ErrorCode = model.ErrorCode;
            result.ErrorDescription = model.ErrorDescription;

            return View(model);
        }

        [HttpPost]
        public IActionResult NotifyLogin([FromBody] OWRequest<NotifyCustomerLoginCall> request)
        {
            var result = serviceProvider.GetRequiredService<NotfityCustomerLoginResult>();
            result.Seq = request.Call?.Seq;

            return Ok(new OWResponse<NotfityCustomerLoginResult>
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
            result.Balance = model.Balance;
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
            if(result.ErrorCode == 0)
            {
                result.ObCustId = request.Call?.ObCustId;
            }

            return Ok(new OWResponse<VoidGameResult>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Result = result
            });
        }
    }
}
