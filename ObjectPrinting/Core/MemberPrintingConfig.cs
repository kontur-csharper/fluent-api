using System;
using ObjectPrinting.Interfaces;

namespace ObjectPrinting.Core
{
    public class MemberPrintingConfig<TOwner, TMemberType> : IMemberPrintingConfig<TOwner>
    {
        private readonly PrintingConfig<TOwner> _parentConfig;
        private readonly string _memberFullName;
        PrintingConfig<TOwner> IMemberPrintingConfig<TOwner>.ParentConfig => _parentConfig;

        public MemberPrintingConfig(PrintingConfig<TOwner> parentConfig, string memberFullName = null)
        {
            _parentConfig = parentConfig;
            _memberFullName = memberFullName;
        }

        public PrintingConfig<TOwner> Using(Func<TMemberType, string> print)
        {
            if (_memberFullName != null)
                ((IPrintingConfig) _parentConfig).AlternativeSerializationByFullName[_memberFullName] =
                    print;
            else
                ((IPrintingConfig) _parentConfig).AlternativeSerializationByTypes[typeof(TMemberType)]
                    = print;
            return _parentConfig;
        }
    }
}