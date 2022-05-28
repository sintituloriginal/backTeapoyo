using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    public class ReporteVisualizacionDocumento
    {
		public string fechaRevision { get; set; }
		public string fechaRevisionFin { get; set; }
	    public string codigoDocumento { get; set; }
		public string nombreDocumento { get; set; }
		public string versionDocumento { get; set; }
		public string tipoActividad { get; set; }
		public string fullNameUsuario { get; set; }
	}
	public class filtros
	{
		//parametros para el listado
		public string value { get; set; }
		public string label { get; set; }

	}