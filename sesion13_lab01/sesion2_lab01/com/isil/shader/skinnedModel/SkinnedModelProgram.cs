using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;

using Device = SharpDX.Direct3D11.Device;
using D3DBuffer = SharpDX.Direct3D11.Buffer;
using D3DMapFlags = SharpDX.Direct3D11.MapFlags;

using Sesion2_Lab01.com.isil.shader.d3d;
using Sesion2_Lab01.com.isil.content;

namespace Sesion2_Lab01.com.isil.shader.skinnedModel {

    public class SkinnedModelProgram {

        private Device mDevice;
        private DeviceContext mDeviceContext;

        private VertexShader mVertexShader;
        private PixelShader mPixelShader;
        private InputLayout mInputLayout;

        private D3DBuffer mMiscInputBuffer;
        private BufferDescription mMiscInputBufferDescription;

        public VertexShader VertexShader    { get { return mVertexShader; } }
        public PixelShader PixelShader      { get { return mPixelShader; } }
        public InputLayout InputLayout      { get { return mInputLayout; } }

        public SkinnedModelProgram(Device device) {
            mDevice = device;
            mDeviceContext = device.ImmediateContext;
        }

        public void Load(string path) {
            CompilationResult vertexShaderByteCode = ShaderBytecode.CompileFromFile(path,
                "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);
            mVertexShader = new VertexShader(mDevice, vertexShaderByteCode);

            // compilamos nuestro fragment shader
            CompilationResult pixelShaderByteCode = ShaderBytecode.CompileFromFile(path,
                "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
            mPixelShader = new PixelShader(mDevice, pixelShaderByteCode);

            ShaderSignature shaderSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
            
            // creamos el contenedor de nuestras variables de entrada
            InputElement[] inputElements = new InputElement[6];
            inputElements[0] = new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0);
            inputElements[1] = new InputElement("NORMAL", 0, Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0);
            inputElements[2] = new InputElement("TEXCOORD", 0, Format.R32G32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0);
            inputElements[3] = new InputElement("TANGENT", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0);
            inputElements[4] = new InputElement("BLENDWEIGHT", 0, Format.R32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0);
            inputElements[5] = new InputElement("BLENDINDICES", 0, Format.R8G8B8A8_UInt, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0);

            // construimos el Input Layout, le pasamos el Shader Signature que contiene la referencia
            // de nuestro Shader, el Input Element que contiene nuestro parametros de entrada, y 
            // mediante esto crea un Objeto para poder enviarlo a la tarjeta de video para poder usarlo
            mInputLayout = new InputLayout(mDevice, shaderSignature, inputElements);

            // ahora creamos una Descripcion para almacenar la informacion de nuestro nuevo bloque
            // de parametros en nuestro Shader
            mMiscInputBufferDescription.Usage = ResourceUsage.Dynamic;
            mMiscInputBufferDescription.BindFlags = BindFlags.ConstantBuffer;
            mMiscInputBufferDescription.CpuAccessFlags = CpuAccessFlags.Write;
            mMiscInputBufferDescription.SizeInBytes = Utilities.SizeOf<SkinnedModelInputParameters>();
            mMiscInputBufferDescription.OptionFlags = ResourceOptionFlags.None;
            
            // creamos un Buffer para almacenar nuestro parametros sueltos
            mMiscInputBuffer = new D3DBuffer(mDevice, mMiscInputBufferDescription);
        }

        public void SetConstantInputParameter<T>(D3DBuffer inputBuffer,
            int subResource, int slot, ref T parametersStruct) {

            // Retrieve the Data Stream from the Shader, so now can we access the Memory Pointer
            // to modify the Input Parameters from the Shader

            DataStream outData = null;
            mDevice.ImmediateContext.MapSubresource(inputBuffer,
                MapMode.WriteDiscard, D3DMapFlags.None, out outData);

            // Now that we set the new values, let's say to the Memory Pointer that we
            // changed the values and update the Memory Pointer
            Marshal.StructureToPtr(parametersStruct, outData.DataPointer, true);

            // Now we re-enable the Buffer
            mDevice.ImmediateContext.UnmapSubresource(inputBuffer, subResource);
            // Now send the Buffer to the Vertex Shader
            mDevice.ImmediateContext.VertexShader.SetConstantBuffer(slot, inputBuffer);
        }

        public void Update(D3DBuffer indexBuffer, VertexBufferBinding vertexBuffer) {
            mDeviceContext.InputAssembler.SetVertexBuffers(0, vertexBuffer);
            mDeviceContext.InputAssembler.SetIndexBuffer(indexBuffer, Format.R16_UInt, 0);
        }

        public void Draw(int indicesCount, int startIndexLocation, SkinnedModelInputParameters inputParameters,
            NTexture2D texture, PrimitiveTopology topology) {

            this.SetConstantInputParameter<SkinnedModelInputParameters>(mMiscInputBuffer,
                0, 0, ref inputParameters);

            mMiscInputBuffer = D3DBuffer.Create<SkinnedModelInputParameters>(mDevice,
                ref inputParameters, mMiscInputBufferDescription);

            //mDeviceContext.VertexShader.SetConstantBuffer(0, mMiscInputBuffer);
            
            mDeviceContext.InputAssembler.InputLayout = mInputLayout;
            // definimos ahora de que manera se va a tratar la data que entra y como se dibuja
            mDeviceContext.InputAssembler.PrimitiveTopology = topology;

            // aqui vamos a transferir el Vertex y Fragment Shader que ya habiamos creado
            mDeviceContext.VertexShader.Set(mVertexShader);
            mDeviceContext.PixelShader.Set(mPixelShader);

            // ahora seteamos nuestra textura en nuestro Shader
            if (texture != null) {
                mDeviceContext.PixelShader.SetShaderResource(0, texture.TextureResource);
                mDeviceContext.PixelShader.SetSampler(0, texture.SamplerState);
            }

            // dibujamos nuestros vertices!
            mDeviceContext.DrawIndexed(indicesCount, startIndexLocation, 0);
        }
    }
}
