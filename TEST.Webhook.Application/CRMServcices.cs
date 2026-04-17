using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TEST.Webhook.Application;

[Route("api/crm")]
public class CRMServcices(IConfiguration configuration) : ApplicationService, ICRMServcices
{
    [AllowAnonymous]
    [HttpGet("hello-world")]
    [HttpHead("hello-world")]
    public string GetHelloWorld()
    {
        Log.Information("---đã được call---");
        return "Hello, KOG CRM API v3.0.1";
    }
}
