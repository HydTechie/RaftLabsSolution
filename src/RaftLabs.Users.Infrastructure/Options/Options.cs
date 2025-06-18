using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftLabs.Users.Infrastructure.Options;

public class ApiOptions
{
    public string BaseUrl { get; set; } 
    public string APIKey { get; set; }  
    public string APIKeyValue { get; set; } 
}
