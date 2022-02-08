

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
using Microsoft.AspNet.SignalR;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Linq;
using MiSmart.Infrastructure.Constants;

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
            var device = (Device)MqttContext.SessionItems[MqttContext.ClientId];
            if (device is not null)
            {
                AddingTelemetryRecordCommand command = null;
                try
                {
                    command = JsonSerializer.Deserialize<AddingTelemetryRecordCommand>(Message.Payload, JsonSerializerDefaultOptions.CamelOptions);
                }
                catch (Exception) { }
                if (command is not null)
                {
                    if (command.Latitude.HasValue && command.Longitude.HasValue && (command.Direction.HasValue && command.Direction >= 0 && command.Direction <= 360))
                    {
                        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                        TelemetryRecord record = new TelemetryRecord
                        {
                            LocationPoint = geometryFactory.CreatePoint(new Coordinate(command.Longitude.GetValueOrDefault(), command.Latitude.GetValueOrDefault())),
                            AdditionalInformation = command.AdditionalInformation,
                            CreatedTime = DateTime.Now,
                            Direction = command.Direction.GetValueOrDefault(),
                            DeviceID = device.ID,

                        };
                        telemetryRecordRepository.Create(record);

                        var device1 = deviceRepository.Get(ww => ww.ID == device.ID);
                        device1.LastPoint = geometryFactory.CreatePoint(new Coordinate(command.Longitude.GetValueOrDefault(), command.Latitude.GetValueOrDefault()));
                        device1.LastDirection = command.Direction.GetValueOrDefault();
                        device1.LastAdditionalInformation = command.AdditionalInformation;
                        deviceRepository.Update(device1);
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
                    command = JsonSerializer.Deserialize<AddingFlightStatCommand>(Message.Payload, JsonSerializerDefaultOptions.CamelOptions);
                }
                catch (Exception)
                {

                }
                if (command is not null)
                {
                    if (command.TaskArea.HasValue && command.Flights.HasValue && command.FlightDuration.HasValue && command.FlywayPoints.Count > 0)
                    {
                        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);


                        var stat = new FlightStat
                        {
                            FlightDuration = command.FlightDuration.GetValueOrDefault(),
                            FieldName = command.FieldName,
                            Flights = command.Flights.GetValueOrDefault(),
                            FlightTime = command.FlightTime ?? DateTime.Now,
                            FlywayPoints = geometryFactory.CreateLineString(command.FlywayPoints.Select(ww => new Coordinate(ww.Longitude.GetValueOrDefault(), ww.Latitude.GetValueOrDefault())).ToArray()),
                            PilotName = command.PilotName,
                            CreatedTime = DateTime.Now,
                            CustomerID = device.CustomerID,
                            DeviceID = device.ID,
                            DeviceName = device.Name,
                            TaskLocation = command.TaskLocation,
                            TaskArea = command.TaskArea.GetValueOrDefault(),

                        };
                        flightStatRepository.Create(stat);
                        if (device.Team is not null)
                        {
                            device.Team.TotalFlights += command.Flights.GetValueOrDefault();
                            device.Team.TotalFlightDuration += command.FlightDuration.GetValueOrDefault();
                            device.Team.TotalTaskArea += command.TaskArea.GetValueOrDefault();
                        }
                        deviceRepository.Update(device);
                        return Ok();
                    }


                }


            }
            return BadMessage();
        }

    }
}