using System.Threading.Tasks;
using MQTTnet.AspNetCore.AttributeRouting;
using MiSmart.DAL.Repositories;

namespace MiSmart.API.MqttControllers
{
    [MqttRoute("[controller]")]

    public class DevicesController : MqttBaseController
    {
        private readonly TelemetryRecordRepository telemetryRecordRepository;
        private readonly FlightStatRepository flightStatRepository;
        private readonly DeviceRepository deviceRepository;

        public DevicesController(TelemetryRecordRepository telemetryRecordRepository, FlightStatRepository flightStatRepository, DeviceRepository deviceRepository) : base()
        {

            this.telemetryRecordRepository = telemetryRecordRepository;
            this.deviceRepository = deviceRepository;
            this.flightStatRepository = flightStatRepository;
        }

        [MqttRoute("me/TelemetryRecords")]
        public Task AddRecord()
        {
            return Ok();
        }

        [MqttRoute("me/FlightStats")]
        public Task AddStat()
        {
            return Ok();
        }

    }
}