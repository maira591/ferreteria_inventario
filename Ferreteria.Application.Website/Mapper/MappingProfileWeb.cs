using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Models.Recaudo;
using Core.Domain.Monitoreo.ViewModel;
using Core.Domain.Recaudo.ViewModel;
using Core.Domain.ViewModel;

namespace Core.Application.Website.Mapping
{
    public class MappingProfileWeb : Profile
    {
        public MappingProfileWeb()
        {
            CreateMap<UserModel, UserBasicModel>().ReverseMap();
            CreateMap<CatalogVM, CatalogModel>().ReverseMap();
            CreateMap<ValueCatalogVM, ValuesCatalogModel>().ReverseMap();
            CreateMap<UserModelComplete, UserBasicModel>().ReverseMap();
            CreateMap<DataAccess.Model.RoleModel, Models.RoleModel>().ReverseMap();
            CreateMap<DataAccess.Model.PrivilegeModel, Models.PrivilegeModel>().ReverseMap();
            CreateMap<FormatColumnVM, FormatColumnModel>().ReverseMap();
            CreateMap<FormatVM, FormatModel>().ReverseMap();
            CreateMap<PeriodicityVM, PeriodicityModel>().ReverseMap();
            CreateMap<CaptureUnitVM, CaptureUnitModel>().ReverseMap();
            CreateMap<FormatLineVM, FormatLineModel>().ReverseMap();
            CreateMap<LoadTypeVM, LoadTypeModel>().ReverseMap();
            CreateMap<ConstantesVM, ConstantesModel>().ReverseMap();
            CreateMap<HomologationVM, HomologationModel>().ReverseMap();
            CreateMap<HomologationValueVM, HomologationValueModel>().ReverseMap();
            CreateMap<QueueMessagePriorityVM, QueueMessagePriorityModel>().ReverseMap();
            CreateMap<MaximunDateVM, MaximunDateModel>().ReverseMap();
            CreateMap<FormatPeriodicityVM, FormatPeriodicityModel>().ReverseMap();
            CreateMap<ReportParameterVM, ReportParameterModel>().ReverseMap();
            CreateMap<ReportParameterDefinitionVM, ReportParameterDefinitionModel>().ReverseMap();
            CreateMap<ReportVM, ReportModel>().ReverseMap();
            CreateMap<ReportPermissionVM, ReportPermissionModel>().ReverseMap();
            CreateMap<ReportColumnVM, ReportColumnModel>().ReverseMap();
            CreateMap<CapitalRequeridoVM, CapitalRequeridoModel>().ReverseMap();
            CreateMap<FormatoVM, FormatoModel>().ReverseMap();
            CreateMap<TasaMoraVM, TasaMoraModel>().ReverseMap();
            CreateMap<TasasVM, TasasModel>().ReverseMap();
            CreateMap<MatcamelBetasVM, MatcamelBetasModel>().ReverseMap();
            CreateMap<RangoSolvAnioVM, RangoSolvAnioModel>().ReverseMap();
            CreateMap<CuentaPucFinanVM, CuentaPucFinanModel>().ReverseMap();
            CreateMap<CuentaPucSolidVM, CuentaPucSolidModel>().ReverseMap();
            CreateMap<RangosValoresVM, RangosValoresModel>().ReverseMap();
            CreateMap<TopesTasasVM, TopesTasasModel>().ReverseMap();
            CreateMap<MatInfoAdicionalVM, MatInfoAdicionalModel>().ReverseMap();
            CreateMap<MonFechaCorteVM, MonFechaCorteModel>().ReverseMap();
            CreateMap<DepartamentosVM, DepartamentosModel>().ReverseMap();
            CreateMap<MunicipiosVM, MunicipiosModel>().ReverseMap();
            CreateMap<PsdControlFechaVM, PsdControlFechaModel>().ReverseMap();
            CreateMap<EntidadVM, EntidadModel>().ReverseMap();
            CreateMap<DashBoardVM, DashBoardModel>().ReverseMap();
            CreateMap<FomatTypeConfigurationVM, FomatTypeConfigurationModel>().ReverseMap();
            CreateMap<GraphicVM, GraphicModel>().ReverseMap();
            CreateMap<GraphicGroupVM, GraphicGroupModel>().ReverseMap();
            CreateMap<GraphicSubGroupVM, GraphicSubGroupModel>().ReverseMap();
            CreateMap<GraphicPermissionVM, GraphicPermissionModel>().ReverseMap();
            CreateMap<GraphicIndicatorVM, GraphicIndicatorModel>().ReverseMap();
            CreateMap<IndicatorVM, IndicatorModel>().ReverseMap();
            CreateMap<AutomaticLogCalculationVM, AutomaticLogCalculationModel>().ReverseMap();
            CreateMap<EntidadesVM, EntidadesModel>().ReverseMap();
        }
    }
}
