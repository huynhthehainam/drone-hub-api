using System;
using System.Collections.Generic;

namespace MiSmart.API.Models
{
    public class GettingAllMedicinesResponse
    {
        public class GettingAllMedicinesResponseData
        {
            public class GettingAllMedicineFromDroneHubModel
            {
                public class GettingAllMedicineFromDroneHubModelData
                {
                    public String ID { get; set; }
                    public String Name { get; set; }
                    public String Code { get; set; }
                    public String Thumbnail { get; set; }

                }
                public List<GettingAllMedicineFromDroneHubModelData> Data { get; set; }
            }
            public GettingAllMedicineFromDroneHubModel GetAllMedicineFromDroneHub { get; set; }
        }
        public GettingAllMedicinesResponseData Data { get; set; }
    }
}