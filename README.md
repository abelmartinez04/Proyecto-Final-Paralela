# Proyecto Final - Programación Paralela en C#

## 📌 Descripción General
Este proyecto consiste en la resolución de un Sudoku utilizando programación paralela en C#. Se implementan dos enfoques:

- Solución secuencial (backtracking tradicional)
- Solución paralela (exploración concurrente de soluciones)

El objetivo principal es demostrar las ventajas y limitaciones del paralelismo aplicando descomposición exploratoria.

---

## 🎯 Objetivos

### Objetivo General
Aplicar técnicas de programación paralela para resolver un problema de búsqueda (Sudoku), evaluando su impacto en el rendimiento.

### Objetivos Específicos
- Implementar una solución secuencial usando backtracking
- Implementar una solución paralela usando TPL
- Comparar tiempos de ejecución
- Analizar speedup y eficiencia
- Evaluar el comportamiento del sistema en distintos niveles de complejidad

---

## 🧠 Concepto Clave: Descomposición Exploratoria
El problema se divide en múltiples tareas que exploran diferentes caminos de solución simultáneamente. Cada tarea prueba posibles valores en el tablero hasta encontrar una solución válida.

---

## 🚀 Ejecución del Proyecto

### Opción 1: Visual Studio
1. Abrir la solución
2. Presionar `F5` o `Run`

### Opción 2: Terminal
```bash
cd src/FinalProject/FinalProject
dotnet run
