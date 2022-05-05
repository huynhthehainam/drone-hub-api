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

    public class GettingAllFarmersResponse
    {
        public class GettingAllFarmersResponseData
        {
            public class GettingAllFarmersResponseDataModel
            {
                public class GettingAllFarmersResponseDataModelData
                {
                    public String ID { get; set; }
                    public String UID { get; set; }
                    public String Name { get; set; }
                    public String Phone { get; set; }
                    public String Email { get; set; }
                }
                public List<GettingAllFarmersResponseDataModelData> Data { get; set; }
            }
            public GettingAllFarmersResponseDataModel GetAllFarmerFromDroneHub { get; set; }
        }
        public GettingAllFarmersResponseData Data { get; set; }
    }
}