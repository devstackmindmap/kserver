using System;
using System.Collections.Generic;

namespace BattleLogic
{
    public static class AttributeCreator
    {
        static AttributeCreator()
        {
            InitTypes();
        }

        private static readonly List<Type> _baseInterfaces = new List<Type>()
        {
            typeof(ISpellSkill)
        };

        private static readonly List<Type> _baseClasses = new List<Type>()
        {
            typeof(BuffSkill),
            typeof(NextBuff),
            typeof(BaseSkillCondition)
        };

        private static Dictionary<Type, List<Type>> _types;

        public static List<Type> GetTypes(Type baseType)
        {
            if (_types == null)
                InitTypes();

            return _types[baseType];
        }

        private static void InitTypes()
        {
            _types = new Dictionary<Type, List<Type>>();
            var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

            for (var i = 0; i < types.Length; i++)
            {
                var containInterface = GetContainTypes(types[i].GetInterfaces());
                if (containInterface != null)
                {
                    AddType(containInterface, types[i]);
                }
                else if (GetContainType(types[i].BaseType))
                {
                    AddType(types[i].BaseType, types[i]);
                }
            }
        }

        private static void AddType(Type baseType, Type addType)
        {
            if(_types.ContainsKey(baseType) == false)
                _types.Add(baseType, new List<Type>());

            _types[baseType].Add(addType);
        }

        private static Type GetContainTypes(Type[] types)
        {
            for (var i = 0; i < types.Length; i++)
            {
                if (_baseInterfaces.Contains(types[i]))
                    return types[i];
            }

            return null;
        }

        private static bool GetContainType(Type type)
        {
            return _baseClasses.Contains(type);
        }

        public static bool IsAttributesEmpty(object[] attributes, Type t)
        {
            return attributes.Length <= 0 || IsAttributeTypeNotEventCode(attributes[0], t);
        }

        private static bool IsAttributeTypeNotEventCode(object attribute, Type t)
        {
            return attribute.GetType() != t;
        }
    }
}