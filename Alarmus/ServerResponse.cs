using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alarmus
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public enum ServerResponse
    {
        SR_NULL,
        SR_AUTORIZATION_SUCCESS,
        SR_AUTORIZATION_FAILED,
        SR_REQUEST_SUCCESS,
        SR_REQUEST_FAILED
    }
}
