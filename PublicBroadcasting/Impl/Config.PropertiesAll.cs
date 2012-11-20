﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicBroadcasting.Impl
{
    internal class PropertiesAllDescriber<T>
    {
        private static readonly PromisedTypeDescription PropertiesAllPromise;
        private static readonly TypeDescription PropertiesAll;

        static PropertiesAllDescriber()
        {
            var promiseType = typeof(PromisedTypeDescription<,>).MakeGenericType(typeof(T), typeof(PropertiesAllDescriber<>).MakeGenericType(typeof(T)));
            var promiseSingle = promiseType.GetField("Singleton");

            PropertiesAllPromise = (PromisedTypeDescription)promiseSingle.GetValue(null);

            var res = Describer.BuildDescription(typeof(PropertiesAllDescriber<>).MakeGenericType(typeof(T)));

            PropertiesAllPromise.Fulfil(res);

            PropertiesAll = res;
        }

        public static IncludedMembers GetMemberMask()
        {
            return IncludedMembers.Properties;
        }

        public static IncludedVisibility GetVisibilityMask()
        {
            return IncludedVisibility.Public | IncludedVisibility.Protected | IncludedVisibility.Internal | IncludedVisibility.Private;
        }

        public static TypeDescription Get()
        {
            // How does this happen you're thinking?
            //   What happens if you call Get() from the static initializer?
            //   That's how.
            return PropertiesAll ?? PropertiesAllPromise;
        }

        public static TypeDescription GetForUse(bool flatten)
        {
            var ret = Get();

            Action postPromise;
            ret = ret.DePromise(out postPromise);
            postPromise();

            ret.Seal();

            if (flatten)
            {
                ret = ret.Clone(new Dictionary<TypeDescription, TypeDescription>());

                Flattener.Flatten(ret, Describer.GetIdProvider());
            }

            return ret;
        }
    }
}