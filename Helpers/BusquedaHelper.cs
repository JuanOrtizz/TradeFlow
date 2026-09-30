namespace TradeFlow.Helpers
{
    // Ordena resultados de busqueda por relevancia: primero las coincidencias exactas,
    // despues las que arrancan con el termino y por ultimo las que solo lo contienen.
    // Dentro de cada grupo se ordena alfabeticamente ignorando mayusculas/minusculas.
    // Todas las comparaciones ignoran mayusculas/minusculas.
    public static class BusquedaHelper
    {
        public const int RangoExacto = 0;
        public const int RangoPrefijo = 1;
        public const int RangoContiene = 2;
        public const int RangoSinCoincidencia = 3;

        public static int ObtenerRango(string? texto, string consulta)
        {
            if (string.IsNullOrWhiteSpace(texto) || string.IsNullOrWhiteSpace(consulta))
                return RangoSinCoincidencia;

            var valor = texto.Trim();
            var termino = consulta.Trim();

            if (valor.Equals(termino, StringComparison.OrdinalIgnoreCase)) return RangoExacto;
            if (valor.StartsWith(termino, StringComparison.OrdinalIgnoreCase)) return RangoPrefijo;
            if (valor.Contains(termino, StringComparison.OrdinalIgnoreCase)) return RangoContiene;

            return RangoSinCoincidencia;
        }

        // Devuelve el mejor rango entre el campo principal y el secundario, para que
        // por ejemplo un producto que coincide exacto por Codigo tambien suba, aunque
        // su Nombre no coincida. El orden alfabetico final siempre usa el campo principal.
        public static IOrderedEnumerable<T> OrdenarPorRelevancia<T>(
            IEnumerable<T> resultados,
            string consulta,
            Func<T, string> campoPrincipal,
            Func<T, string?>? campoSecundario = null)
        {
            return resultados
                .OrderBy(x => Math.Min(
                    ObtenerRango(campoPrincipal(x), consulta),
                    campoSecundario is null ? RangoSinCoincidencia : ObtenerRango(campoSecundario(x), consulta)))
                .ThenBy(x => campoPrincipal(x), StringComparer.OrdinalIgnoreCase);
        }
    }
}
