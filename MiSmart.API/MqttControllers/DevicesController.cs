

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiSmart.API.Services;
using MiSmart.DAL.Models;
using MiSmart.API.Commands;
using MiSmart.Infrastructure.Mqtt;
using MiSmart.Infrastructure.Responses;
using MQTTnet.AspNetCore.AttributeRouting;
using System.Text.Json;
using MiSmart.DAL.Repositories;
using System.Collections.Generic;

namespace MiSmart.API.MqttControllers
{

    [MqttController]
    [MqttRoute("[controller]")]

    public class DevicesController : MqttBaseController
    {
        private readonly TelemetryRecordRepository telemetryRecordRepository;
        private readonly FlightStatRepository flightStatRepository;

        public DevicesController(TelemetryRecordRepository telemetryRecordRepository, FlightStatRepository flightStatRepository) : base()
        {

            this.telemetryRecordRepository = telemetryRecordRepository;
            this.flightStatRepository = flightStatRepository;


        }

        [MqttRoute("me/TelemetryRecords")]
        public Task AddRecord()
        {
            var device = (Device)MqttContext.SessionItems[MqttContext.ClientId];
            if (device is not null)
            {
                AddingTelemetryRecordCommand command = null;
                try
                {
                    command = JsonSerializer.Deserialize<AddingTelemetryRecordCommand>(Message.Payload, JsonOptions.CamelOptions);
                }
                catch (Exception) { }
                if (command is not null)
                {
                    if (command.Latitude.HasValue && command.Longitude.HasValue)
                    {
                        TelemetryRecord record = new TelemetryRecord
                        {
                            Latitude = command.Latitude.GetValueOrDefault(),
                            Longitude = command.Longitude.GetValueOrDefault(),
                            AdditionalInformation = command.AdditionalInformation,
                            CreatedTime = DateTime.Now,
                            DeviceID = device.ID,

                        };
                        telemetryRecordRepository.Create(record);
                        return Ok();
                    }
                }

            }
            return BadMessage();
        }

        [MqttRoute("me/FlightStats")]
        public Task AddStat()
        {
            var device = (Device)MqttContext.SessionItems[MqttContext.ClientId];
            if (device is not null)
            {
                AddingFlightStatCommand command = null;
                try
                {
                    command = JsonSerializer.Deserialize<AddingFlightStatCommand>(Message.Payload, JsonOptions.CamelOptions);
                }
                catch (Exception)
                {

                }
                if (command is not null)
                {
                    if (command.TaskArea.HasValue && command.Flights.HasValue && command.FlightDuration.HasValue)
                    {
                        var stat = new FlightStat
                        {
                            FlightDuration = command.FlightDuration.GetValueOrDefault(),
                            FieldName = command.FieldName,
                            Flights = command.Flights.GetValueOrDefault(),
                            FlightTime = command.FlightTime ?? DateTime.Now,
                            FlywayPoints = command.FlywayPoints,
                            PilotName = command.PilotName,
                            CreateTime = DateTime.Now,
                            CustomerID = device.CustomerID,
                            DeviceID = device.ID,
                            DeviceName = device.Name,
                            TaskLocation = command.TaskLocation,
                            TaskArea = command.TaskArea.GetValueOrDefault(),
                            TaskAreaUnit = command.TaskAreaUnit,

                        };
                        flightStatRepository.Create(stat);
                        return Ok();
                    }
                }


            }
            return BadMessage();
        }

    }
}