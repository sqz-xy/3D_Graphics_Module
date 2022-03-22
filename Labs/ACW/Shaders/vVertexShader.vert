#version 330

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

uniform vec3 uLightDirection;
uniform vec4 uLightPosition;

in vec3 vPosition; 
in vec3 vNormal;
in vec2 vTexCoords;

out vec2 oTexCoords;
out vec4 oColour;

void main() 
{ 
	oTexCoords = vTexCoords;
	gl_Position = vec4(vPosition, 1) * uModel * uView * uProjection; 

	vec3 inverseTransposeNormal = normalize(vNormal * mat3(transpose(inverse(uModel * uView))));
	vec3 lightDir = normalize(-uLightDirection * mat3(uView));

	vec4 directionalLight = vec4(vec3(max(dot(inverseTransposeNormal, lightDir), 0)), 1);

	vec4 surfacePosition = vec4(vPosition, 1) * uModel * uView; 
	vec4 lightPosition = vec4(uLightPosition) * uView; 
	vec4 posLightDir = normalize(lightPosition - surfacePosition); 

	vec4 positionalLight = vec4(vec3(max(dot(vec4(inverseTransposeNormal, 1), posLightDir), 0)), 1); 
	
	oColour = positionalLight;
	//oColour = positionalLight + directionalLight;
}