﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicBroadcasting.Impl
{
    internal class AllInternalPrivateDescriber<T>
    {
        private static readonly PromisedTypeDescription AllInternalPrivatePromise;
        private static readonly TypeDescription AllInternalPrivate;

        static AllInternalPrivateDescriber()
        {
            var promiseType = typeof(PromisedTypeDescription<,>).MakeGenericType(typeof(T), typeof(AllInternalPrivateDescriber<>).MakeGenericType(typeof(T)));
            var promiseSingle = promiseType.GetField("Singleton");

            AllInternalPrivatePromise = (PromisedTypeDescription)promiseSingle.GetValue(null);

            var res = Describer.BuildDescription(typeof(AllInternalPrivateDescriber<>).MakeGenericType(typeof(T)));

            AllInternalPrivatePromise.Fulfil(res);

            AllInternalPrivate = res;
        }

        public static IncludedMembers GetMemberMask()
        {
            return IncludedMembers.Fields | IncludedMembers.Properties;
        }

        public static IncludedVisibility GetVisibilityMask()
        {
            return IncludedVisibility.Internal | IncludedVisibility.Private;
        }

        public static TypeDescription Get()
        {
            // How does this happen you're thinking?
            //   What happens if you call Get() from the static initializer?
            //   That's how.
            return AllInternalPrivate ?? AllInternalPrivatePromise;
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
