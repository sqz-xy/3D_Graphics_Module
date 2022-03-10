#version 330

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform vec3 uLightPosition;

in vec3 vPosition; 
in vec3 vNormal;

out vec4 oColour;

void main() 
{ 
	gl_Position = vec4(vPosition, 1) * uModel * uView * uProjection; 
	vec3 inverseTransposeNormal = normalize(vNormal * mat3(transpose(inverse(uModel * uView))));
	vec4 surfacePosition = vec4(vPosition, 1) * uModel * uView;
	vec4 lightPosition = vec4(uLightPosition, 1) * uView;
	vec4 lightDir = normalize(lightPosition - surfacePosition);
	oColour = vec4(vec3(max(dot(vec4(inverseTransposeNormal, 1), lightDir), 0)), 1);
}