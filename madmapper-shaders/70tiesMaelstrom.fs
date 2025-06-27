/*{
	"CREDIT": "by mojovideotech",
	"CATEGORIES": ["noise", "Automatically Converted"],
	"DESCRIPTION": "Modified version with customizable parameters.",
	"INPUTS": [
		{
			"NAME": "speed",
			"TYPE": "float",
			"MIN": 0.1,
			"MAX": 2.0,
			"DEFAULT": 0.5
		},
		{
			"NAME": "amplitude",
			"TYPE": "float",
			"MIN": 0.1,
			"MAX": 2.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorShift",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 6.28,
			"DEFAULT": 1.0
		},
		{
			"NAME": "rotationSpeed",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 2.0,
			"DEFAULT": 0.5
		},
		{
			"NAME": "noiseScale",
			"TYPE": "float",
			"MIN": 0.5,
			"MAX": 3.0,
			"DEFAULT": 1.2
		},
		{
			"NAME": "layerDensity",
			"TYPE": "float",
			"MIN": 3.0,
			"MAX": 10.0,
			"DEFAULT": 7.0
		},
		{
			"NAME": "detailLevel",
			"TYPE": "float",
			"MIN": 0.1,
			"MAX": 1.5,
			"DEFAULT": 1.0
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"MIN": 0.5,
			"MAX": 2.0,
			"DEFAULT": 1.0
		}
	]
}*/

#define PI 3.1415927
#define MAX_ITER 10  // Fixed max iterations for WebGL compatibility

const mat3 m = mat3( 0.00,  0.80,  0.60,
                     -0.80,  0.36, -0.48,
                     -0.60, -0.48,  0.64 );

float hash( float n ) {
    return fract(sin(n) * 43758.5453);
}

float noise( in vec3 x ) {
    vec3 p = floor(x);
    vec3 f = fract(x);

    f = f * f * (3. - 2. * f);

    float n = p.x + p.y * 57. + 113. * p.z;

    float res = mix(mix(mix( hash(n +   0.), hash(n +   1.), f.x),
                        mix( hash(n +  57.), hash(n +  58.), f.x), f.y),
                    mix(mix( hash(n + 113.), hash(n + 114.), f.x),
                        mix( hash(n + 170.), hash(n + 171.), f.x), f.y), f.z);
    return res;
}

float fbm( vec3 p ) {
    float f;
    f  = 0.5000 * noise(p); p = m * p * 2.02;
    f += 0.2500 * noise(p); p = m * p * 2.03;
    f += 0.1250 * noise(p); p = m * p * 2.01;
    f += 0.0625 * noise(p);
    return f;
}

vec2 sfbm2( vec3 p ) {
    return 2. * vec2(fbm(p), fbm(p - 327.67)) - 1.;
}

void main()
{
    float t = TIME * speed;
    vec2 uv = 2. * (gl_FragCoord.xy / RENDERSIZE.y - vec2(.9, .5));
    
    float a = rotationSpeed * TIME;
    float c = cos(a), s = sin(a);
    mat2 rotMat = mat2(c, -s, s, c);

    vec4 col = vec4(0.);
    vec3 paint = vec3(.3, .9, .7);
    
    for(int i = 0; i < MAX_ITER; i++) {
        float z = float(i) / layerDensity;
        if (z >= 1.0) break; // Exit when reaching the limit

        paint = 0.5 + 0.5 * cos(colorShift + 4. * 2. * PI * z + vec3(0., 2. * PI / 3., -2. * PI / 3.));
        uv *= rotMat;
        vec2 duv = vec2(amplitude, 0.5) * sfbm2(vec3(noiseScale * uv, 3. * z + t)) - 3. * z * vec2(0.5, 0.5);
        float d = length(uv + duv) - 1.2 * (1. - z),
              a = smoothstep(.1, .09 * detailLevel, abs(d)); 
        
        d = 0.5 * a + 0.5 * smoothstep(.09, .08 * detailLevel, abs(d));
        col += (1. - col.a) * vec4(d * paint * exp(-0. * z) * brightness, a);
        
        if (col.a >= .9) break;
    }
	gl_FragColor = col;
}
