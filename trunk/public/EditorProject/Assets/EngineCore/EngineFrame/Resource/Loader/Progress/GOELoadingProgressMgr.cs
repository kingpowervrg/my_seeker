//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace GOEngine.Implement
//{
//#if UNITY_EDITOR
//    public
//#else
//    internal
//#endif
//        class LoadingProgressMgr : ResComponent
//    {
//        private List<LoadingProgress> mListProgress = new List<LoadingProgress>();

//        public int AddProgress( LoadingProgress pro )
//        {
//            mListProgress.Add( pro );
//            pro.Start();
//            pro.Update();
//            return pro.GetHashCode();
//        }

//        public int GetLoadingCount()
//        {
//            return mListProgress.Count;
//        }

//        public void DelProgress( int id )
//        {
//            for ( int i = 0; i < mListProgress.Count; ++i )
//            {
//                LoadingProgress pro = mListProgress[i];
//                if ( pro.GetHashCode() == id )
//                {
//                    mListProgress.RemoveAt( i );
//                    break;
//                }
//            }
//        }

//        public LoadingProgress GetLoadingProgress( int id )
//        {
//            for ( int i = 0; i < mListProgress.Count; ++i )
//            {
//                LoadingProgress pro = mListProgress[i];
//                if ( pro.GetHashCode() == id )
//                {
//                    return pro; 
//                }
//            }

//            return null;
//        }

//        public LoadingProgress GetLoadingProgressByIndex( int index )
//        {
//            return mListProgress[index];
//        }


//        internal override void Update()
//        {
//            base.Update();
			
//            for( int i = 0; i < mListProgress.Count; i++ )
//            {
//                LoadingProgress pro = mListProgress[i];
//                pro.Update();
//            }
//        }
//    }
//}
