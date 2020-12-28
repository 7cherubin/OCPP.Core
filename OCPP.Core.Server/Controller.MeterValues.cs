﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OCPP.Core.Database;
using OCPP.Core.Server.Messages;

namespace OCPP.Core.Server
{
    public partial class Controller
    {
        public string HandleMeterValues(Message msgIn, Message msgOut)
        {
            string errorCode = null;
            MeterValuesResponse meterValuesResponse = new MeterValuesResponse();

            int? connectorId = null;

            try
            {
                Logger.LogTrace("Processing meter values...");
                MeterValuesRequest meterValueRequest = JsonConvert.DeserializeObject<MeterValuesRequest>(msgIn.JsonPayload);
                Logger.LogTrace("MeterValues => Message deserialized");

                connectorId = meterValueRequest.ConnectorId;

                if (CurrentChargePoint != null)
                {
                    // Known charge station

                    foreach (MeterValue meterValue in meterValueRequest.MeterValue)
                    {
                        foreach (SampledValue sampleValue in meterValue.SampledValue)
                        {
                            //sampleValue.
                            Logger.LogTrace("MeterValues => Context={0} / Format={1} / Value={2} / Unit={3} / Location={4} / Measurand={5} / Phase={6}",
                                sampleValue.Context, sampleValue.Format, sampleValue.Value, sampleValue.Unit, sampleValue.Location, sampleValue.Measurand, sampleValue.Phase);
                        }
                    }

                }
                else
                {
                    // Unknown charge station
                    errorCode = ErrorCodes.GenericError;
                }

                msgOut.JsonPayload = JsonConvert.SerializeObject(meterValuesResponse);
                Logger.LogTrace("MeterValues => Response serialized");
            }
            catch (Exception exp)
            {
                Logger.LogError(exp, "MeterValues => Exception: {0}", exp.Message);
                errorCode = ErrorCodes.InternalError;
            }

            WriteMessageLog(CurrentChargePoint.ChargePointId, connectorId, msgIn.Action, null, errorCode);
            return errorCode;
        }
    }
}
