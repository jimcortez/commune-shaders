/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Original: sqrt_1, Port of Humus Electro demo)",
	"DESCRIPTION": "Creates an electro effect with simplex noise and glow parameters. Features a port of the Humus Electro demo with customizable controls for size, burn, wiggle, and freak parameters that generate dynamic electrical plasma-like visual effects.",
	"IMPORTED": {
	},
	"INPUTS": [
		{
			"DEFAULT": 0.16,
			"LABEL": "Mid size 1",
			"MAX": 2,
			"MIN": 0,
			"NAME": "Slider1",
			"TYPE": "float"
		},
		{
			"DEFAULT": 0.15,
			"LABEL": "Mid size 2",
			"MAX": 0.4,
			"MIN": 0,
			"NAME": "Slider2",
			"TYPE": "float"
		},
		{
			"DEFAULT": 0.2,
			"LABEL": "Burn",
			"MAX": 1,
			"MIN": 0,
			"NAME": "Slider3",
			"TYPE": "float"
		},
		{
			"DEFAULT": 52,
			"LABEL": "Wiggle amp",
			"MAX": 100,
			"MIN": 0,
			"NAME": "Slider4",
			"TYPE": "float"
		},
		{
			"DEFAULT": 0.5,
			"LABEL": "Freak",
			"MAX": 10,
			"MIN": 0.5,
			"NAME": "Slider5",
			"TYPE": "float"
		},
		{
			"DEFAULT": 0.6,
			"LABEL": "Freak2",
			"MAX": 1,
			"MIN": 0,
			"NAME": "Slider6",
			"TYPE": "float"
		}
	],
	"ISFVSN": "2"
}*/

/*
ORIGINAL SHADER INFORMATION:
- Modified by kosh
- Original Author: sqrt_1 (https://www.shadertoy.com/view/4scGWj)
- Port of: Humus Electro demo (http://humus.name/index.php?page=3D&ID=35)
- Simplex noise by: Nikita Miropolskiy (https://www.shadertoy.com/view/XsX3zB)
- Source: Originally sourced from editor.isf.video - Electro by sqrt_1 (https://editor.isf.video/shaders/6404aa7d54062a0019bd6df5)
- Description: Electro effect with simplex noise and glow parameters
- License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License
- Features: Multiple slider controls for size, burn, wiggle, and freak parameters
*/

/* Simplex code license
 * This work is licensed under a 
 * Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License
 * http://creativecommons.org/licenses/by-nc-sa/3.0/
 *  - You must attribute the work in the source code 
 *    (link to https://www.shadertoy.com/view/XsX3zB).
 *  - You may not use this work for commercial purposes.
 *  - You may distribute a derivative work only under the same license.
 */

/* discontinuous pseudorandom uniformly distributed in [-0.5, +0.5]^3 */
vec3 random3(vec3 c) {
	float j = 4096.0*sin(dot(c,vec3(17.0, 59.4, 15.0)));
	vec3 r;
	r.z = fract(512.0*j);
	j *= .125;
	r.x = fract(512.0*j);
	j *= .125;
	r.y = fract(512.0*j);
	return r-Slider5;
}

/* skew constants for 3d simplex functions */
const float F3 =  0.3333333;
const float G3 =  0.1666667;

/* 3d simplex noise */
float simplex3d(vec3 p) {
	 /* 1. find current tetrahedron T and it's four vertices */
	 /* s, s+i1, s+i2, s+1.0 - absolute skewed (integer) coordinates of T vertices */
	 /* x, x1, x2, x3 - unskewed coordinates of p relative to each of T vertices*/
	 
	 /* calculate s and x */
	 vec3 s = floor(p + dot(p, vec3(F3)));
	 vec3 x = p - s + dot(s, vec3(G3));
	 
	 /* calculate i1 and i2 */
	 vec3 e = step(vec3(0.0), x - x.yzx);
	 vec3 i1 = e*(1.0 - e.zxy);
	 vec3 i2 = 1.0 - e.zxy*(1.0 - e);
	 	
	 /* x1, x2, x3 */
	 vec3 x1 = x - i1 + G3;
	 vec3 x2 = x - i2 + 2.0*G3;
	 vec3 x3 = x - 1.0 + 3.0*G3;
	 
	 /* 2. find four surflets and store them in d */
	 vec4 w, d;
	 
	 /* calculate surflet weights */
	 w.x = dot(x, x);
	 w.y = dot(x1, x1);
	 w.z = dot(x2, x2);
	 w.w = dot(x3, x3);
	 
	 /* w fades from 0.6 at the center of the surflet to 0.0 at the margin */
	 w = max(Slider6 - w, 0.0);
	 
	 /* calculate surflet components */
	 d.x = dot(random3(s), x);
	 d.y = dot(random3(s + i1), x1);
	 d.z = dot(random3(s + i2), x2);
	 d.w = dot(random3(s + 1.0), x3);
	 
	 /* multiply d by w^4 */
	 w *= w;
	 w *= w;
	 d *= w;
	 
	 /* 3. return the sum of the four surflets */
	 return dot(d, vec4(Slider4));
}

float noise(vec3 m) {
    return   0.5333333*simplex3d(m)
			+0.2666667*simplex3d(2.0*m)
			+0.1333333*simplex3d(4.0*m)
			+0.0666667*simplex3d(8.0*m);
}

void main() {



  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;    
  uv = uv * 2. -1.;  
 
  vec2 p = gl_FragCoord.xy/RENDERSIZE.x;
  vec3 p3 = vec3(p, TIME*0.4);    
    
  float intensity = noise(vec3(p3*12.0+12.0));
                          
  float t = clamp((uv.x * -uv.x * Slider1) + Slider2, 0., 1.);                         
  float y = abs(intensity * -t + uv.y);
    
  float g = pow(y, Slider3);
                          
  vec3 col = vec3(1.70, 1.48, 1.78);
  col = col * -g + col;                    
  col = col * col;
  col = col * col;
                          
  gl_FragColor.rgb = col;                          
  gl_FragColor.w = 1.;  
}
