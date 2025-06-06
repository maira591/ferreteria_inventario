﻿using System;

namespace Ferreteria.DataAccess.Core
{
    public abstract class Auditable
    {
        public string CreatedBy
        {
            get;
            set;
        }

        public DateTime CreatedOn
        {
            get;
            set;
        }

        public string UpdatedBy
        {
            get;
            set;
        }

        public DateTime? UpdatedOn
        {
            get;
            set;
        }

        protected Auditable()
        {

        }
    }
}
