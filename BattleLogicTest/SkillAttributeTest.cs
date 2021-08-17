using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BattleLogicTest
{
    public class SkillAttributeTest
    {
        [Test]
        public void BaseSkillTest()
        {
            var types = GetTypes(typeof(TestBaseSkill));
            Assert.AreNotEqual(types.Count, 0);
        }

        [Test]
        public void InterfaceSkillTest()
        {
            var types = GetTypes(typeof(ISkill));
            Assert.AreEqual(types.Count, 0);
        }

        [Test]
        public void FindInterfaceTest()
        {
            var types = new List<Type>();
            var assemblyTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            for (var i = 0; i < assemblyTypes.Length; i++)
            {
                foreach (var type in assemblyTypes[i].GetInterfaces())
                {
                    var skillType = typeof(ISkill);
                    if (type == skillType)
                    {
                        types.Add(type);
                    }
                }
            }

            Assert.AreNotEqual(types.Count, 0);
        }

        private Dictionary<Type, List<Type>> GetTypes(Type type)
        {
            var types = new Dictionary<Type, List<Type>>();
            var assemblyTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            for (var i = 0; i < assemblyTypes.Length; i++)
            {
                if (assemblyTypes[i].BaseType == type)
                {
                    if (types.ContainsKey(assemblyTypes[i].BaseType) == false)
                        types.Add(assemblyTypes[i].BaseType, new List<Type>());
                    types[assemblyTypes[i].BaseType].Add(assemblyTypes[i]);
                }
            }

            return types;
        }

        public class TestBaseSkill
        {
            public int Damage;
        }

        public class TestChildSkill1 : TestBaseSkill
        {
        }

        public class TestChildSkill2 : TestBaseSkill
        {
        }

        public interface ISkill
        {
            int Damage { get; set; }
        }

        public class TestInterfaceSkill1 : ISkill
        {
            public int Damage { get; set; }
        }

        public class TestInterfaceSkill2 : ISkill
        {
            public int Damage { get; set; }
        }
    }
}