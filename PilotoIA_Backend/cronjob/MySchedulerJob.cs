using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using PilotoIA_Backend.BusinessLogic;
using PilotoIA_Backend.Models;
using PilotoIA_Backend.DataAccess;
using PilotoIA_Backend.Controllers;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using Microsoft.Extensions.Options;

namespace PilotoIA_Backend.cronjob
{
    public class MySchedulerJob : CronBackgroundJob
    {
        private readonly ILogger<MySchedulerJob> _log;

        private beMySettings vgSettings;

        private CronJobDAO cronjobVgDataAccess;

        public MySchedulerJob(CronSettings<MySchedulerJob> settings, ILogger<MySchedulerJob> log, IOptions<beMySettings> peSettings)
            : base(settings.CronExpression, settings.TimeZone)
        {
            _log = log;

            this.vgSettings = peSettings.Value;
            cronjobVgDataAccess = new CronJobDAO(vgSettings.DbConnection);
        }

        protected override async Task DoWork(CancellationToken stoppingToken)
        {
            _log.LogInformation("Running... at {0}", DateTime.UtcNow);

            await cronjobVgDataAccess.ActualizarEstadosAsync();
            await StopAsync();
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }

    }
}
