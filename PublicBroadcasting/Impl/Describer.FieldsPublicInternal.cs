﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicBroadcasting.Impl
{
    internal class FieldsPublicInternalDescriber<T>
    {
        private static readonly PromisedTypeDescription FieldsPublicInternalPromise;
        private static readonly TypeDescription FieldsPublicInternal;

        static FieldsPublicInternalDescriber()
        {
            var promiseType = typeof(PromisedTypeDescription<,>).MakeGenericType(typeof(T), typeof(FieldsPublicInternalDescriber<>).MakeGenericType(typeof(T)));
            var promiseSingle = promiseType.GetField("Singleton");

            FieldsPublicInternalPromise = (PromisedTypeDescription)promiseSingle.GetValue(null);

            var res = Describer.BuildDescription(typeof(FieldsPublicInternalDescriber<>).MakeGenericType(typeof(T)));

            FieldsPublicInternalPromise.Fulfil(res);

            FieldsPublicInternal = res;
        }

        public static IncludedMembers GetMemberMask()
        {
            return IncludedMembers.Fields;
        }

        public static IncludedVisibility GetVisibilityMask()
        {
            return IncludedVisibility.Public | IncludedVisibility.Internal;
        }

        public static TypeDescription Get()
        {
            // How does this happen you're thinking?
            //   What happens if you call Get() from the static initializer?
            //   That's how.
            return FieldsPublicInternal ?? FieldsPublicInternalPromise;
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