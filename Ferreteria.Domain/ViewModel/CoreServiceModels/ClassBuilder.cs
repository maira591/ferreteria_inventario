﻿using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Ferreteria.Domain.ViewModel.CoreServiceModels
{
    public class ClassBuilder
    {
        AssemblyName asemblyName;
        public ClassBuilder(string ClassName)
        {
            this.asemblyName = new AssemblyName(ClassName);
        }
        public object CreateObject(string[] PropertyNames, Type[] Types, bool[] columnsIsRequired)
        {
            if (PropertyNames.Length != Types.Length)
            {
                Console.WriteLine("The number of property names should match their corresponding types number");
            }

            TypeBuilder DynamicClass = this.CreateClass();
            this.CreateConstructor(DynamicClass);
            for (int ind = 0; ind < PropertyNames.Count(); ind++)
                CreateProperty(DynamicClass, PropertyNames[ind], Types[ind]);
            Type type = DynamicClass.CreateType();

            return Activator.CreateInstance(type);
        }

        public object CreateObject(string[] PropertyNames, string[] typeProperties, bool[] columnsIsRequired)
        {

            TypeBuilder DynamicClass = this.CreateClass();
            this.CreateConstructor(DynamicClass);
            for (int ind = 0; ind < PropertyNames.Count(); ind++)
                CreateProperty(DynamicClass, PropertyNames[ind], GetTypeByNameProperty(typeProperties[ind], columnsIsRequired[ind]));
            Type type = DynamicClass.CreateType();

            return Activator.CreateInstance(type);
        }


        private static Type GetTypeByNameProperty(string nameProperty, bool columnIsRequired)
        {
            return nameProperty switch
            {
                "VARCHAR2" => typeof(string),
                "DATE" => columnIsRequired ? typeof(DateTime?) : typeof(DateTime),
                "NUMBER" => columnIsRequired ? typeof(decimal?) : typeof(decimal),
                _ => typeof(string),
            };
        }


        private TypeBuilder CreateClass()
        {
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(this.asemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(this.asemblyName.FullName
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , null);
            return typeBuilder;
        }
        private void CreateConstructor(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }
        private void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }

}
