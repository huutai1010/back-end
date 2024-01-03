using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class ChartResponse<T> : BaseResponse
    {
        public T Chart { get; set; }
        public ChartResponse(T chart) : base()
        {
            Chart = chart;  
        }
    }

    public class ChartListResponse<T> : BaseResponse
    {
        public T Charts { get; set; }
        public decimal TotalRevenue { get; set; }
        public ChartListResponse(T data, decimal totalRevenue)
        {
            Charts = data;
            TotalRevenue = totalRevenue;
        }
        public ChartListResponse(T data)
        {
            Charts = data;
        }
    }

    public class StaticticalResponse<T> : BaseResponse
    {
        public T Statictical { get; set; }
        public StaticticalResponse(T data)
        {
            Statictical = data;
        }
    }

    public class StaticticalNationalResponse<T> : BaseResponse
    {
        public T Statictical { get; set; }
        public int total { get; set; }
        public StaticticalNationalResponse(T data, int total)
        {
            Statictical = data;
            this.total = total;
        }
    }

    public class OrderStaticticalResponse<T> : BaseResponse
    {
        public T StaticticalOrder { get; set; }
        public OrderStaticticalResponse(T data)
        {
            StaticticalOrder = data;
        }
    }
}
