using Common.Models.RabbitMq;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IRabbitmqService
    {
        bool PublidMessage(List<FileMessageModel> fileMessageModels);
    }
}
