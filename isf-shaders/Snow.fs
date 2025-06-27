/*{
	"CATEGORIES": [],
	"CREDIT": "Jim Cortez - Commune Project (Original: Unknown, inspired by Shadertoy)",
	"DESCRIPTION": "Creates an animated snow effect with realistic falling snowflakes. Features speed and density controls for customizable snowfall patterns with gradient sky background.",
	"INPUTS": [
		{"NAME": "speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0},
		{"NAME": "density", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 12.0}
	],
	"ISFVSN": "2",
	"PASSES": [
		{"TARGET": "bufferVariableNameA", "WIDTH": "$WIDTH/16.0", "HEIGHT": "$HEIGHT/16.0"},
		{"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"}
	]
}*/

/*
ORIGINAL SHADER INFORMATION:
- Original Author: Unknown (inspired by Shadertoy)
- Source: Originally sourced from editor.isf.video - Snow by Unknown
- Original Source: https://www.shadertoy.com/view/Mdt3Df
- Description: Animated snow effect with realistic falling snowflakes
- License: Unknown (Shadertoy)
- Features: Animated snow, density and speed controls
*/

void main()
{
    float snow = 0.0;
    float gradient = (1.0-float(gl_FragCoord.y / RENDERSIZE.y))*0.99;
    float random = fract(sin(dot(gl_FragCoord.xy,vec2(12.9898,78.233)))* 43758.5453);
    float time = TIME * speed;
    for(int k=0;k<12;k++){
        for(int i=0;i<14;i++){
            float cellSize = 2.0 + (float(i)*3.0);
			float downSpeed = 0.3+(sin(time*0.4+float(k+i*20))+1.0)*0.00008;
            vec2 uv = (gl_FragCoord.xy / RENDERSIZE.x)+vec2(0.01*sin((time+float(k*6185))*0.6+float(i))*(5.0/float(i)),downSpeed*(time+float(k*1352))*(1.0/float(i)));
            vec2 uvStep = (ceil((uv)*cellSize-vec2(0.5,0.5))/cellSize);
            float x = fract(sin(dot(uvStep.xy,vec2(12.9898+float(k)*12.0,78.233+float(k)*315.156)))* 43758.5453+float(k)*12.0)-0.5;
            float y = fract(sin(dot(uvStep.xy,vec2(62.2364+float(k)*23.0,94.674+float(k)*95.0)))* 62159.8432+float(k)*12.0)-0.5;

            float randomMagnitude1 = sin(time*2.5)*0.7/cellSize;
            float randomMagnitude2 = cos(time*2.5)*0.7/cellSize;

            float d = 5.0*distance((uvStep.xy + vec2(x*sin(y),y)*randomMagnitude1 + vec2(y,x)*randomMagnitude2),uv.xy);

            float omiVal = fract(sin(dot(uvStep.xy,vec2(32.4691,94.615)))* 31572.1684);
            if(omiVal<0.08?true:false){
                float newd = (x+1.0)*0.4*clamp(1.9-d*(15.0+(x*6.3))*(cellSize/1.4),0.0,1.0);
                /*snow += d<(0.08+(x*0.3))/(cellSize/1.4)?
                    newd
                    :newd;*/
                snow += newd;
            }
        }
    }
    
    
    gl_FragColor = vec4(snow)+gradient*vec4(0.4,0.7,1.0,1.0) + random*0.01;
}
