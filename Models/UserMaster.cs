using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace digi_sante.Models
{
    public class UserMaster
    {
       public long ID { get; set; }
       public string Name { get; set; }
       public string EmailID { get; set; }
       public string MobileNo { get; set; }
    }
}