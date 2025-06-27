# Commune Shaders

A collection of custom shaders created for **Commune**, an art project by Jim Cortez. This repository contains interactive shaders in multiple formats for use in various creative applications.

Many of these shaders were sourced from https://editor.isf.video/shaders, which has very poor handling of licenses. When applicable, I tried to keep the licenses and references to the source shader intact. If I omitted anything, it was not on purpose. If you have an issue with a license or credit, please open an issue or reach out in a message and I can quickly address it.

## About

Commune is an art project that explores visual expression through real-time graphics and interactive media. These shaders are designed to create immersive, dynamic visual experiences that respond to user input and environmental parameters.

## Project Structure

The repository is organized into three main directories, each containing shaders optimized for different platforms and use cases:

### `/isf-shaders/`
Interactive Shader Format (ISF) shaders for use in applications like:
- xLights
- Resolume
- CoGe
- Other ISF-compatible software

**Key shaders include:**
- `Aurora.fs` - Ethereal aurora borealis effects
- `Acid Jelly.fs` - Organic, flowing acid-like animations
- `CosmicFlare.fs` - Space-inspired light effects
- `Hex 3D Spiral.fs` - Geometric spiral patterns
- `MolecularWatercolor.fs` - Organic molecular structures
- `Snowflakes.fs` - Procedural snowflake generation
- `Voronoi Spiral Vortex.fs` - Mathematical voronoi patterns

### `/madmapper-shaders/`
Shaders specifically adapted for MadMapper, a projection mapping software:
- Optimized for real-time projection mapping
- Enhanced parameter controls for live performance
- Compatible with MadMapper's shader system

### `/madmapper-materials/`
Material shaders for MadMapper that provide:
- Surface texture effects
- Lighting and reflection properties
- Material-specific visual behaviors

## Usage

### ISF Shaders
1. Load the `.fs` files into any ISF-compatible application
2. Adjust parameters in real-time for live performance
3. Use the corresponding `.json` files for metadata and parameter definitions

### MadMapper Shaders
1. Import `.fs` files directly into MadMapper
2. Apply to surfaces or objects in your projection mapping setup
3. Use the material shaders for surface effects and textures

## Technical Details

- **Language**: GLSL (OpenGL Shading Language)
- **Format**: Fragment shaders (.fs files)
- **Compatibility**: ISF 2.0+, MadMapper 4.0+
- **Performance**: Optimized for real-time rendering

## Artist

**Jim Cortez** - Creator of the Commune art project and these custom shaders.

## License

This project is part of the Commune art installation. The use of these shaders is free to all with no license. Many shaders have been adapted from sources online. When possible, I have tried to maintain a link or reference to the original authors.

---

*These shaders are designed to create immersive, interactive visual experiences that blur the line between digital art and live performance.* 