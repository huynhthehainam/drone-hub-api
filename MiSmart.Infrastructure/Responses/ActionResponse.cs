using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using MiSmart.Infrastructure.Data;
using System.Text.Json;

namespace MiSmart.Infrastructure.Responses
{
    public class ActionResponseSettings
    {
        public ResponseLanguage Language { get; set; }
    }
    public class ErrMessage
    {
        public String Name { get; set; }
        public List<String> Errs { get; set; } = new List<String>();
    }
    public enum ResponseType
    {
        File,
        Json
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResponseLanguage
    {
        EN,
        VI
    }
    public class ErrorException
    {
        public Int32 StatusCode { get; set; }
        public Object Errors { get; set; }
    }

    public class ActionResponse
    {
        public ActionResponse()
        {
        }
        public void ApplySettings(ActionResponseSettings settings)
        {
            this.Language = settings.Language;
        }
        public Int32? TotalItems { get; set; }

        public String Type
        {
            get
            {
                if (errMessages.Count > 0)
                    return "Detect error by MiSmart";
                return null;
            }
        }
        public String Title
        {
            get
            {
                if (errMessages.Count > 0)
                    return "One or more validation errors occurred.";
                return null;
            }
        }
        [JsonIgnore]
        public ResponseLanguage Language { get; set; } = ResponseLanguage.EN;
        protected Byte[] bytes;
        protected String contentType;
        protected String fileName;
        protected ResponseType responseType = ResponseType.Json;

        public void SetFile(Byte[] bytes, String contentType, String fileName)
        {
            this.bytes = bytes;
            this.contentType = contentType;
            this.fileName = fileName;
            this.responseType = ResponseType.File;
        }

        public void SetData(Object data)
        {
            this.Data = data;
            this.responseType = ResponseType.Json;
        }
        public Int32 StatusCode { get; set; } = 200;
        public Object Data { get; set; }
        public OrderedDictionary Errors
        {
            get
            {
                if (errMessages.Count == 0)
                    return null;
                OrderedDictionary errors = new OrderedDictionary();
                foreach (var errMessage in errMessages)
                {
                    errors.Add(errMessage.Name, errMessage.Errs);
                }
                return errors;
            }
        }

        public String Message { get; set; }
        public void SetMessage(String enMessage = "", String viMessage = "")
        {
            if (Language == ResponseLanguage.EN)
                Message = enMessage;
            else
                Message = viMessage;
        }
        public void SetUpdatedMessage()
        {
            SetMessage("Updated", "Đã thay đổi");
        }
        public void SetCreatedObject<T>(EntityBase<T> createdObject)
        {
            this.Data = new { ID = createdObject.ID };
            this.StatusCode = 201;
        }
        private List<ErrMessage> errMessages = new List<ErrMessage>();
        private ErrMessage GetErrMessage(String name)
        {
            ErrMessage errMessage = errMessages.FirstOrDefault(dd => dd.Name == name);
            if (errMessage == null)
            {
                errMessage = new ErrMessage() { Name = name };
                errMessages.Add(errMessage);
            }
            return errMessage;
        }
        public void AddMessageErr(String name, String enMessage = "", String viMessage = "", Int32 statusCode = 400, Boolean raiseException = true)
        {
            StatusCode = statusCode;
            ErrMessage errMessage = GetErrMessage(name);
            if (Language == ResponseLanguage.EN)
                errMessage.Errs.Add(enMessage);
            else
                errMessage.Errs.Add(viMessage);
            if (raiseException)
                RaiseException();
        }

        public void RaiseException()
        {
            throw new Exception(JsonSerializer.Serialize(new ErrorException() { StatusCode = this.StatusCode, Errors = this.Errors }));
        }
        public void AddRequirementErr(String name, Boolean raiseException = true)
        {
            AddMessageErr(name, $"The {name} field is required", $"Trường {name} bị thiếu", 400, raiseException);
        }
        public void AddNotAllowedErr(Boolean raiseException = true)
        {

            AddMessageErr("Permission", $"Your permission's denied", $"Không có quyền truy cập", 403, raiseException);
        }
        public void AddNotFoundErr(String name, Boolean raiseException = true)
        {
            AddMessageErr(name, $"The {name} field's not found", $"Trường {name} không được tìm thấy", 404, raiseException);
        }
        public void AddAuthorizationErr(Boolean raiseException = true)
        {
            AddMessageErr("Authorization", $"The authorization field's invalid", $"Không được xác thực", 404, raiseException);

        }
        public void SetNoContent()
        {
            StatusCode = 204;
            Data = null;
        }
        public void AddExpiredErr(String name, Boolean raiseException = true)
        {
            AddMessageErr(name, $"The {name} field exceeds expiring time", $"Trường {name} vượt quá thời gian cho phép", 400, raiseException);
        }
        public void AddInvalidErr(String name, Boolean raiseException = true)
        {
            AddMessageErr(name, $"The {name} field's invalid", $"Trường {name} không hợp lệ", 400, raiseException);
        }
        public void AddExistedErr(String name, Boolean raiseException = true)
        {
            AddMessageErr(name, $"The {name} field already exists", $"Trường {name} đã được tồn tại", 400, raiseException);
        }
        public IActionResult ToIActionResult()
        {
            switch (responseType)
            {
                case ResponseType.Json:
                    {
                        return new ObjectResult(new { TotalItems = TotalItems, Type = Type, Title = Title, Data = Data, Errors = Errors, Message = Message }) { StatusCode = this.StatusCode };
                    }
                case ResponseType.File:
                    {
                        var result = new FileContentResult(bytes, contentType);
                        result.FileDownloadName = fileName;
                        return result;
                    }
                default:
                    {
                        return new ObjectResult(new { TotalItems = TotalItems, Type = Type, Title = Title, Data = Data, Errors = Errors, Message = Message }) { StatusCode = this.StatusCode };
                    }
            }
        }
    }
}