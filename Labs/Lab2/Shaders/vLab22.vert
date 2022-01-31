#version 330

in vec3 vPosition;
in vec3 vColour;

out vec4 oColour;

void main()
{
	gl_Position = vec4(vPosition, 1);
	oColour = vec4(vColour, 1);
}