﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.core.Entities
{
    public class Excel
    {
        public byte[] FileContents { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }
    }
}
