#version 330

uniform mat4 uModel;
uniform mat4 uView;

in vec3 vPosition;
in vec3 vColour;

out vec4 oColour;

void main()
{
	gl_Position = vec4(vPosition, 1) * uModel;
	oColour = vec4(vColour, 1);
	gl_Position = vec4(vPosition, 1) * uModel * uView;
}