using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.IServices
{
    public interface IUrlService
    {
        string GetServerUrl();
        string GetFullUrl(string? path);
    }
}
