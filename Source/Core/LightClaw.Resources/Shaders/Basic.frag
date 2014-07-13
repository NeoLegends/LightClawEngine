#version 400

in vec3 inColor;
in vec3 inPosition;

out vec3 passColor;

void main(void)
{
	gl_Position = vec4(inPosition, 1.0);
	passColor = inColor;
}